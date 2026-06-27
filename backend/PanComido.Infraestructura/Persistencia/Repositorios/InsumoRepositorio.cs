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

            // 2. Procesamos el vencimiento más cercano en memoria (C#)
            foreach (var articulo in efLista)
            {
                var proximoVencimiento = articulo.Insumo.Lotes
                    .Where(l => l.FechaVencimiento != null)
                    .Min(l => l.FechaVencimiento);

                if (proximoVencimiento != null)
                {
                    // Como ya incluimos los Lotes arriba, el mapper ahora sí sumará el StockActual
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
    }
}
