using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
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
        public async Task ActualizarAsync(DOM.Articulo articulo)
        {
            var efArticuloNuevo = _mapper.paraEntidad(articulo);

            var articuloDB = await _ctx.Articulos
                .Include(a => a.ConfiguracionArticulos)
                .Include(a => a.Plato)
                .Include(a => a.Insumo)
                .FirstOrDefaultAsync(a => a.Id == articulo.Id);

            if (articuloDB != null)
            {
                if (efArticuloNuevo.Plato != null)
                    efArticuloNuevo.Plato.IdArticulo = articuloDB.Id;

                if (efArticuloNuevo.Insumo != null)
                    efArticuloNuevo.Insumo.IdArticulo = articuloDB.Id;

                _ctx.Entry(articuloDB).CurrentValues.SetValues(efArticuloNuevo);

                if (articuloDB.Plato != null && efArticuloNuevo.Plato != null)
                {
                    _ctx.Entry(articuloDB.Plato).CurrentValues.SetValues(efArticuloNuevo.Plato);
                }

                if (articuloDB.Insumo != null && efArticuloNuevo.Insumo != null)
                {
                    _ctx.Entry(articuloDB.Insumo).CurrentValues.SetValues(efArticuloNuevo.Insumo);
                }

                var configVisible = await _ctx.Set<EF.ConfiguracionArticulo>().FindAsync((int)ConfiguracionArticuloEnum.VisibleEnCarta);
                if (configVisible != null)
                {
                    var yaEraVisible = articuloDB.ConfiguracionArticulos.Any(c => c.Id == (int)ConfiguracionArticuloEnum.VisibleEnCarta);

                    if (articulo.EsVisibleEnCarta && !yaEraVisible)
                    {
                        articuloDB.ConfiguracionArticulos.Add(configVisible);
                    }
                    else if (!articulo.EsVisibleEnCarta && yaEraVisible)
                    {
                        var configARemover = articuloDB.ConfiguracionArticulos.First(c => c.Id == (int)ConfiguracionArticuloEnum.VisibleEnCarta);
                        articuloDB.ConfiguracionArticulos.Remove(configARemover);
                    }
                }

                await _ctx.SaveChangesAsync();
            }
        }

        public async Task<List<DOM.Articulo>> ObtenerArticulosEnCartaConIngredientesAsync(int restauranteId)
        {
            List<EF.Articulo> articulosEnCarta = await _ctx.Articulos
            .AsNoTracking()
            .Include(a => a.ConfiguracionArticulos)
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
                     && a.ConfiguracionArticulos.Any(c => c.Id == (int)ConfiguracionArticuloEnum.VisibleEnCarta)) 
            .ToListAsync();

            return articulosEnCarta.Select(a => _mapper.paraDominio(a)).ToList();
        }

        public async Task<DOM.Articulo> ObtenerDetalleAsync(int restauranteId, int articuloId)
        {
            var efArticulo = await _ctx.Articulos
                .AsNoTracking()
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

        public async Task<List<Articulo>> ObtenerTodosLosArticulosParaCartaAsync(int restauranteId)
        {
          var efArticulos = await _ctx.Articulos
                .AsNoTracking()
                .Include(a => a.ConfiguracionArticulos)
                
                .Include(a => a.Plato)
                    .ThenInclude(p => p.CategoriaPlato)
                .Include(a => a.Plato)
                    .ThenInclude(p => p.PlatoIngredientes)
                        .ThenInclude(pi => pi.Ingrediente)
                            .ThenInclude(ing => ing.IdInsumoNavigation)
                                .ThenInclude(ins => ins.PedidoInsumos)
                                    .ThenInclude(pi => pi.Pedido)
                .Include(a => a.Insumo)
                    .ThenInclude(i => i.CategoriaInsumo)
                .Include(a => a.Insumo)
                    .ThenInclude(i => i.PedidoInsumos)
                        .ThenInclude(pi => pi.Pedido)
                .Include(a => a.Plato)
                    .ThenInclude(p => p.Restriccions) 
                .Where(a => a.RestauranteId == restauranteId 
                         && !a.Eliminado 
                         && (a.Plato != null || (a.Insumo != null && a.Insumo.CategoriaInsumo.TipoAplica == (int)TipoInsumo.Bebida)))
                .ToListAsync();

            return efArticulos.Select(a => _mapper.paraDominio(a)).ToList();

        }
    }
}
