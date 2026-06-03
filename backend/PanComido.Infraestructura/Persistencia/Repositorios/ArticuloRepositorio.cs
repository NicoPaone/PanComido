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
                    .ThenInclude(p => p.TipoPlato)
            .Include(a => a.Plato)
                .ThenInclude(p => p.Restriccions)
            .Where(a => a.RestauranteId == restauranteId
                     && a.CartaId != null) 
            .ToListAsync();

            return articulosEnCarta.Select(a => _mapper.paraDominio(a)).ToList();
        }

        public async Task<DOM.Articulo> ObtenerDetalleAsync(int restauranteId, int articuloId)
        {
            var efArticulo = await _ctx.Articulos
                .Include(a => a.Insumo)
                    .ThenInclude(i => i.CategoriaInsumo)
                .Include(a => a.Plato)
                    .ThenInclude(p => p.PlatoIngredientes) 
                        .ThenInclude(pi => pi.Ingrediente) 
                            .ThenInclude(ing => ing.IdInsumoNavigation) 
                                .ThenInclude(i => i.IdArticuloNavigation)
                .Include(a => a.Plato)
                    .ThenInclude(p => p.CategoriaPlato)
                .Include(a => a.Plato)
                    .ThenInclude(p => p.TipoPlato)
                .Include(a => a.Plato)
                    .ThenInclude(p => p.Restriccions)
                .Include(a => a.ConfiguracionArticulos)
                .FirstOrDefaultAsync(a => a.Id == articuloId && a.RestauranteId == restauranteId);

            if (efArticulo == null) return null;

            return _mapper.paraDominio(efArticulo);
        }

        public async Task<List<Articulo>> ObtenerTodosLosArticulosParaCartaAsync()
        {
          var efArticulos = await _ctx.Articulos
                .AsNoTracking()
                .Include(a => a.ConfiguracionArticulos)
                // --- INCLUDES PARA PLATOS ---
                .Include(a => a.Plato)
                    .ThenInclude(p => p.CategoriaPlato)
                .Include(a => a.Plato)
                    .ThenInclude(p => p.PlatoIngredientes)
                        .ThenInclude(pi => pi.Ingrediente)
                            .ThenInclude(ing => ing.IdInsumoNavigation)
                                .ThenInclude(ins => ins.PedidoInsumos) // <-- CAMBIO ACÁ
                                                                       // --- INCLUDES PARA INSUMOS (BEBIDAS) ---
                .Include(a => a.Insumo)
                    .ThenInclude(i => i.CategoriaInsumo)
                .Include(a => a.Insumo)
                    .ThenInclude(i => i.PedidoInsumos) // <-- CAMBIO ACÁ
                .Where(a => a.Plato != null || (a.Insumo != null && a.Insumo.CategoriaInsumo.TipoAplica == 2))
                .ToListAsync();

            // Usamos tu mapper inyectado para devolver la lista de Dominio
            return efArticulos.Select(a => _mapper.paraDominio(a)).ToList();

        }
    }
}
