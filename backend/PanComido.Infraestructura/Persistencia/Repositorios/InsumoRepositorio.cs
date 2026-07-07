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
                     && a.Insumo != null)
            .Include(a => a.Insumo)
                .ThenInclude(i => i.CategoriaInsumo)
            .Include(a => a.Insumo)
                .ThenInclude(i => i.UnidadMedida)
            .Include(a => a.Insumo)
                .ThenInclude(i => i.Ingrediente)
                    .ThenInclude(ing => ing.IngredientePreparado);

        public async Task<List<DOM.Insumo>> ObtenerInsumosAsync(int restauranteId)
        {
            var efLista = await BaseQuery(restauranteId).ToListAsync();
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

        public async Task<DOM.Insumo> CrearAsync(DOM.Insumo insumoDominio)
        {
            EF.Articulo efArticulo = _mapper.paraEntidad(insumoDominio);
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
                .FirstOrDefaultAsync(a => a.Id == insumoId);

            return efArticulo == null ? null : (DOM.Insumo)_mapper.paraDominio(efArticulo);
        }

        public async Task ActualizarAsync(DOM.Insumo insumoDominio)
        {
            var efArticulo = await BaseQuery(insumoDominio.RestauranteId)
                .FirstOrDefaultAsync(a => a.Id == insumoDominio.Id);

            if (efArticulo == null)
            {
                throw new InvalidOperationException("Insumo no encontrado para actualizar.");
            }

            ActualizarDatosBasicos(efArticulo, insumoDominio);

            await _ctx.SaveChangesAsync();
        }

        public async Task EliminarAsync(int insumoId, int restauranteId)
        {
            var efArticulo = await _ctx.Articulos
                .FirstOrDefaultAsync(a => a.Id == insumoId && a.RestauranteId == restauranteId && a.Insumo != null);

            if (efArticulo == null)
            {
                throw new KeyNotFoundException("El insumo no existe o no pertenece al restaurante.");
            }

            efArticulo.Eliminado = true;
            await _ctx.SaveChangesAsync();
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
