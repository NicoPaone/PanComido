using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class PlatoAnalisisRepositorio : IPlatoAnalisisRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly ArticuloEntityMapper _mapper;

        public PlatoAnalisisRepositorio(AppDbContext ctx, ArticuloEntityMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }

        public async Task<DOM.Articulo?> ObtenerArticuloConPlatoYIngredientesPorNombreAsync(int restauranteId, string nombre)
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
                .FirstOrDefaultAsync(a => a.Nombre.ToLower() == nombre.ToLower() 
                                       && a.RestauranteId == restauranteId 
                                       && !a.Eliminado);

            if (efArticulo == null) return null;

            return _mapper.paraDominio(efArticulo);
        }

        public async Task<decimal> ObtenerUltimoPrecioCompraInsumoAsync(int insumoId)
        {
            return await _ctx.PedidoInsumos
                .Where(pi => pi.InsumoId == insumoId)
                .OrderByDescending(pi => pi.Pedido.Fecha)
                .Select(pi => pi.PrecioCompra)
                .FirstOrDefaultAsync();
        }

        public async Task<int> ObtenerVentasArticuloEnRangoAsync(int restauranteId, int articuloId, DateTime desde, DateTime hasta)
        {
            return await _ctx.ArticuloComanda
                .Where(ac => ac.ArticuloId == articuloId
                          && ac.Comanda.RestauranteId == restauranteId
                          && ac.Comanda.HoraInicio >= desde
                          && ac.Comanda.HoraInicio <= hasta
                          && ac.Comanda.Pagos.Any())
                .SumAsync(ac => (int?)ac.Cantidad) ?? 0;
        }

        public async Task<int> ObtenerVentasCategoriaEnRangoAsync(int restauranteId, int categoriaPlatoId, DateTime desde, DateTime hasta)
        {
            return await _ctx.ArticuloComanda
                .Where(ac => ac.Articulo.RestauranteId == restauranteId
                          && ac.Articulo.Plato != null
                          && ac.Articulo.Plato.CategoriaPlatoId == categoriaPlatoId
                          && ac.Comanda.HoraInicio >= desde
                          && ac.Comanda.HoraInicio <= hasta
                          && ac.Comanda.Pagos.Any())
                .SumAsync(ac => (int?)ac.Cantidad) ?? 0;
        }

        public async Task<DOM.RendimientoPlato?> ObtenerPlatoLiderDeCategoriaAsync(int restauranteId, int categoriaPlatoId, DateTime desde, DateTime hasta)
        {
            var query = _ctx.Articulos
                .Where(a => a.RestauranteId == restauranteId 
                         && a.Plato != null 
                         && a.Plato.CategoriaPlatoId == categoriaPlatoId 
                         && !a.Eliminado)
                .Select(a => new DOM.RendimientoPlato
                {
                    PlatoId = a.Id,
                    Nombre = a.Nombre,
                    UnidadesVendidas = _ctx.ArticuloComanda
                        .Where(ac => ac.ArticuloId == a.Id 
                                  && ac.Comanda.RestauranteId == restauranteId
                                  && ac.Comanda.HoraInicio >= desde
                                  && ac.Comanda.HoraInicio <= hasta
                                  && ac.Comanda.Pagos.Any())
                        .Sum(ac => (int?)ac.Cantidad) ?? 0,
                    FacturacionTotal = _ctx.ArticuloComanda
                        .Where(ac => ac.ArticuloId == a.Id 
                                  && ac.Comanda.RestauranteId == restauranteId
                                  && ac.Comanda.HoraInicio >= desde
                                  && ac.Comanda.HoraInicio <= hasta
                                  && ac.Comanda.Pagos.Any())
                        .Sum(ac => (decimal?)(ac.Cantidad * (a.PrecioVentaFinal ?? 0m))) ?? 0m
                });

            return await query
                .OrderByDescending(p => p.UnidadesVendidas)
                .FirstOrDefaultAsync();
        }

        public async Task GuardarRecordatorioNotificacionAsync(int restauranteId, string descripcion)
        {
            var notificacion = new EF.Notificacion
            {
                RestauranteId = restauranteId,
                Fecha = DateTime.UtcNow,
                Descripcion = descripcion,
                Resuelta = false
            };

            await _ctx.Notificacions.AddAsync(notificacion);
            await _ctx.SaveChangesAsync();
        }

        public async Task<List<DOM.Notificacion>> ObtenerRecordatoriosActivosAsync(int restauranteId)
        {
            var efNotificaciones = await _ctx.Notificacions
                .Where(n => n.RestauranteId == restauranteId 
                         && !n.Resuelta 
                         && n.Descripcion.StartsWith("Revisión: "))
                .ToListAsync();

            return efNotificaciones.Select(n => new DOM.Notificacion
            {
                Id = n.Id,
                RestauranteId = n.RestauranteId,
                Fecha = n.Fecha,
                Descripcion = n.Descripcion,
                Resuelta = n.Resuelta
            }).ToList();
        }

        public async Task ResolverNotificacionAsync(int restauranteId, int id)
        {
            var notificacion = await _ctx.Notificacions
                .FirstOrDefaultAsync(n => n.RestauranteId == restauranteId && n.Id == id);
            if (notificacion != null)
            {
                notificacion.Resuelta = true;
                await _ctx.SaveChangesAsync();
            }
        }
    }
}
