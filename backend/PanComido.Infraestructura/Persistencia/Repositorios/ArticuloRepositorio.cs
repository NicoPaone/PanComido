using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
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
    public class ArticuloRepositorio : IArticuloRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly ArticuloEntityMapper _mapper;

        public ArticuloRepositorio(AppDbContext ctx, ArticuloEntityMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }

        public async Task<List<DOM.Articulo>> ObtenerArticulosEnCartaConIngredientesAsync(int restauranteId)
        {
            List<EF.Articulo> articulosEnCarta = await _ctx.Articulos
            .AsNoTracking()
            .Include(a => a.Insumo)
                .ThenInclude(i => i.CategoriaInsumo)
            .Include(a => a.Insumo)
            .Include(a => a.Plato)
                .ThenInclude(p => p.PlatoIngredientes)
            .Include(a => a.Plato)
                .ThenInclude(p => p.CategoriaPlato)
            .Include(a => a.Plato)
                .ThenInclude(p => p.Restriccions)
            .Where(a => a.RestauranteId == restauranteId
                     && a.CartaId != null) 
            .ToListAsync();

            return articulosEnCarta.Select(a => _mapper.paraDominio(a)).ToList();
        }
    }
}
