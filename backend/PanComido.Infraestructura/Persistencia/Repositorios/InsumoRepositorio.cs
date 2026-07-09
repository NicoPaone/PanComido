using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Entidades;
using PanComido.Infraestructura.Persistencia.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class InsumoRepositorio : IInsumoRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly ArticuloEntityMapper _mapper;

        public InsumoRepositorio(AppDbContext context, ArticuloEntityMapper mapper)
        {
            _ctx = context;
            _mapper = mapper;
        }

        private IQueryable<EF.Articulo> BaseQuery(int restauranteId) => _ctx.Articulos
            .Where(a => a.RestauranteId == restauranteId
                     && a.Insumo != null
                     && !a.Eliminado)
            .Include(a => a.Insumo)
                .ThenInclude(i => i.CategoriaInsumo)
            .Include(a => a.Insumo)
                .ThenInclude(i => i.UnidadMedida)
            .Include(a => a.Insumo)
                .ThenInclude(i => i.Ingrediente)
                    .ThenInclude(ing => ing.IngredientePreparado)
            .Include(a => a.ConfiguracionArticulos);

        public async Task<List<DOM.Insumo>> ObtenerInsumosAsync(int restauranteId)
        {
            var efLista = await BaseQuery(restauranteId)
                .Include(a => a.Insumo)
                    .ThenInclude(i => i.PedidoInsumos)
                        .ThenInclude(pi => pi.Pedido)
                .ToListAsync();
            return efLista.Select(a => (DOM.Insumo)_mapper.paraDominio(a)).ToList();
        }

        public async Task<List<DOM.Insumo>> ObtenerInsumosConLotesAsync(int restauranteId)
        {
            var efLista = await BaseQuery(restauranteId)
                                .Include(a => a.Insumo)
                                    .ThenInclude(i => i.Lotes).ToListAsync();

            return efLista.Select(a => (DOM.Insumo)_mapper.paraDominio(a)).ToList();
        }

        public async Task<List<DOM.Insumo>> ObtenerInsumosProximosAVencerAsync(int restauranteId)
        {
            // 1. Agregamos el Include de los Lotes y quitamos el .Select()
            var efLista = await BaseQuery(restauranteId)
                .Include(a => a.Insumo)
                    .ThenInclude(i => i.Lotes)
                .Where(a => a.Insumo != null && a.Insumo.Lotes.Any(l => l.FechaVencimiento != null))
                .ToListAsync();

            var domLista = new List<DOM.Insumo>();

            // 2. Procesamos el vencimiento m�s cercano en memoria (C#)
            foreach (var articulo in efLista)
            {
                var proximoVencimiento = articulo.Insumo.Lotes
                    .Where(l => l.FechaVencimiento != null)
                    .Min(l => l.FechaVencimiento);

                if (proximoVencimiento != null)
                {
                    // Como ya incluimos los Lotes arriba, el mapper ahora s� sumar� el StockActual
                    var domInsumo = (DOM.Insumo)_mapper.paraDominio(articulo);
                    domInsumo.Vencimiento = proximoVencimiento;
                    domLista.Add(domInsumo);
                }
            }

            // 3. Ordenamos por vencimiento y retornamos
            return domLista.OrderBy(x => x.Vencimiento).ToList();
        }

        public async Task<List<DOM.Insumo>> ObtenerInsumosDelProveedorAsync(int proveedorId, int restauranteId)
        {
            var efLista = await BaseQuery(restauranteId)
                .Where(a =>
                    a.Insumo.CategoriaInsumo.Proveedors.Any(p => p.Id == proveedorId)
                    && (a.Insumo.Ingrediente == null || a.Insumo.Ingrediente.IngredientePreparado == null))
                .ToListAsync();

            return efLista.Select(a => (DOM.Insumo)_mapper.paraDominio(a)).ToList();
        }

        public async Task<bool> ExisteInsumoConNombreAsync(int restauranteId, string nombre)
        {
            return await _ctx.Articulos
                .AnyAsync(a => a.RestauranteId == restauranteId
                            && a.Insumo != null
                            && !a.Eliminado
                            && a.Nombre.ToLower() == nombre.ToLower());
        }

        public async Task<bool> ExisteInsumoAsync(int insumoId, int restauranteId)
        {
            return await _ctx.Articulos
                .AnyAsync(a => a.Id == insumoId 
                            && a.RestauranteId == restauranteId 
                            && a.Insumo != null 
                            && !a.Eliminado);
        }

        public async Task<DOM.Insumo> CrearAsync(DOM.Insumo insumoDominio)
        {
            EF.Articulo efArticulo = _mapper.paraEntidad(insumoDominio);
            await ConfiguracionVisibilidadHelper.AplicarVisibilidadEnCartaAsync(_ctx, efArticulo, insumoDominio.EsVisibleEnCarta);

            await _ctx.Articulos.AddAsync(efArticulo);

            await _ctx.SaveChangesAsync();

            insumoDominio.Id = efArticulo.Id;

            return insumoDominio;
        }

        public async Task<DOM.Insumo> ObtenerPorIdAsync(int insumoId, int restauranteId)
        {
            var efArticulo = await BaseQuery(restauranteId)
                .Include(a => a.Insumo)
                    .ThenInclude(i => i.Lotes)
                .Include(a => a.Insumo)
                    .ThenInclude(i => i.PedidoInsumos)
                        .ThenInclude(pi => pi.Pedido)
                .FirstOrDefaultAsync(a => a.Id == insumoId);

            return efArticulo == null ? null : (DOM.Insumo)_mapper.paraDominio(efArticulo);
        }

        public async Task<DOM.Insumo> ActualizarAsync(DOM.Insumo insumoDominio)
        {
            var efArticulo = await BaseQuery(insumoDominio.RestauranteId)
                .FirstOrDefaultAsync(a => a.Id == insumoDominio.Id);

            if (efArticulo == null) return null;

            ActualizarDatosBasicos(efArticulo, insumoDominio);
            await ConfiguracionVisibilidadHelper.AplicarVisibilidadEnCartaAsync(_ctx, efArticulo, insumoDominio.EsVisibleEnCarta);

            await _ctx.SaveChangesAsync();
            return insumoDominio;
        }

        public async Task<DOM.Insumo> EliminarAsync(int insumoId, int restauranteId)
        {
            var efArticulo = await _ctx.Articulos
                .FirstOrDefaultAsync(a => a.Id == insumoId && a.RestauranteId == restauranteId && a.Insumo != null);

            if (efArticulo == null) return null;

            efArticulo.Eliminado = true;
            await _ctx.SaveChangesAsync();

            return new DOM.Insumo { Id = efArticulo.Id, Nombre = efArticulo.Nombre };
        }

        private void ActualizarDatosBasicos(EF.Articulo efArticulo, DOM.Insumo insumoDominio)
        {
            efArticulo.Nombre = insumoDominio.Nombre;
            efArticulo.Descripcion = insumoDominio.Descripcion;
            efArticulo.PrecioVentaFinal = insumoDominio.PrecioVentaFinal;
            efArticulo.UrlImagen = insumoDominio.UrlImagen;
            efArticulo.EsPrecioManual = insumoDominio.EsPrecioManual;
            efArticulo.Insumo.CategoriaInsumoId = insumoDominio.CategoriaId;
            efArticulo.Insumo.UnidadMedidaId = insumoDominio.UnidadDeMedidaId;
            efArticulo.Insumo.StockMinimo = insumoDominio.StockMinimo;
            efArticulo.Insumo.StockRecomendado = insumoDominio.StockRecomendado;
        }
    }
}
