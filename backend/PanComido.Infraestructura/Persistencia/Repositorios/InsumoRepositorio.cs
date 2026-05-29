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

        public async Task<List<DOM.Insumo>> ObtenerInsumosAsync(int restauranteId)
        {
            var efLista = await BaseQuery(restauranteId).ToListAsync();
            return efLista.Select(a => _mapper.paraDominio(a)).ToList();
        }

        public async Task<List<DOM.Insumo>> ObtenerInsumosDelProveedorAsync(int proveedorId, int restauranteId)
        {
            var efLista = await BaseQuery(restauranteId)
                .Where(a =>
                    (a.Insumo.Ingrediente != null
                     && a.Insumo.Ingrediente.IngredientePreparado == null
                     && a.Insumo.Ingrediente.CategoriaIngrediente
                           .CategoriaProveedors
                           .Any(cp => cp.Proveedors.Any(p => p.Id == proveedorId)))
                    ||
                    (a.Insumo.Bebidum != null
                     && _ctx.CategoriaProveedors
                           .Any(cp => cp.Descripcion == "Bebidas"
                                   && cp.Proveedors.Any(p => p.Id == proveedorId))))
                .ToListAsync();

            return efLista.Select(a => _mapper.paraDominio(a)).ToList();
        }
    }
}
