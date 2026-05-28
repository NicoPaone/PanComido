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

        // query base: articulos que tienen fila en insumo
        private IQueryable<EF.Articulo> BaseQuery(int restauranteId) => _ctx.Articulos
        // 1. Filtramos usando a.Insumo
        .Where(a => a.RestauranteId == restauranteId
                 && a.Insumo != null)

        // 2. Incluimos usando a.Insumo y encadenamos con Ingrediente
        .Include(a => a.Insumo)
            .ThenInclude(i => i.Ingrediente)
                .ThenInclude(ing => ing.CategoriaIngrediente)

        // 3. Volvemos a encadenar para sacar la UnidadMedida del Ingrediente
        .Include(a => a.Insumo)
            .ThenInclude(i => i.Ingrediente)
                .ThenInclude(ing => ing.UnidadMedida)

        // 4. Encadenamos usando a.Insumo y Bebidum
        .Include(a => a.Insumo)
            .ThenInclude(i => i.Bebidum)
                .ThenInclude(b => b.CategoriaBebida);
        public Task<DOM.Insumo> ObtenerInsumoPorIdAsync(int restauranteId, int idInsumo)
        {
            throw new NotImplementedException();
        }

        public async Task<List<DOM.Insumo>> ObtenerInsumosAsync(int restauranteId)
        {
            var efLista = await BaseQuery(restauranteId).ToListAsync();
            return efLista.Select(a => _mapper.paraDominio(a)).ToList();
        }

        public Task<List<DOM.Insumo>> ObtenerInsumosPorBusquedaAsync(int restauranteId, string busqueda)
        {
            throw new NotImplementedException();
        }

        public Task<List<DOM.Insumo>> ObtenerInsumosPorCategoriaAsync(int restauranteId, string categoria)
        {
            throw new NotImplementedException();
        }
    }
}
