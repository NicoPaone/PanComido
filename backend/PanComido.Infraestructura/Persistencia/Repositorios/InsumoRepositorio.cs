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
        private readonly InsumoEntityMapper _mapper;

        public InsumoRepositorio(AppDbContext context, InsumoEntityMapper mapper)
        {
            _ctx = context;
            _mapper = mapper;
        }

        // Consulta base donde obtenemos articulos que son insumos, con toda su info relacionada
        // (ingrediente, categoria ingrediente, unidad medida, bebida y categoria bebida)
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
            return efLista.Select(a => _mapper.paraDominio(a)).ToList();
        }

        public async Task<List<DOM.Insumo>> ObtenerInsumosDelProveedorAsync(int proveedorId, int restauranteId)
        {
            var efLista = await BaseQuery(restauranteId)
                .Where(a =>
                    a.Insumo.CategoriaInsumo.Proveedors.Any(p => p.Id == proveedorId)
                    && (a.Insumo.Ingrediente == null || a.Insumo.Ingrediente.IngredientePreparado == null))
                .ToListAsync();

            return efLista.Select(a => _mapper.paraDominio(a)).ToList();
        }
    }
}
