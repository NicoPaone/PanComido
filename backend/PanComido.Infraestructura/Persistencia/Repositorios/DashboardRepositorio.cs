using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class DashboardRepositorio : IDashboardRepositorio
    {
        private readonly AppDbContext _ctx;

        public DashboardRepositorio(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<TotalesPeriodo> ObtenerTotalesPeriodoAsync(int restauranteId, DateTime desde, DateTime hasta)
        {
            var query = _ctx.Comanda
                .Where(c => c.RestauranteId == restauranteId
                         && c.HoraInicio >= desde
                         && c.HoraInicio <= hasta
                         && c.Pagos.Any());

            var result = await query
                .GroupBy(c => 1)
                .Select(g => new TotalesPeriodo
                {
                    CantidadPedidos = g.Count(),
                    TotalFacturado = g.Sum(c => c.Pagos.Sum(p => p.Total))
                })
                .FirstOrDefaultAsync();

            return result ?? new TotalesPeriodo { CantidadPedidos = 0, TotalFacturado = 0 };
        }

        public async Task<List<VentaAgrupada>> ObtenerVentasAgrupadasAsync(int restauranteId, DateTime desde, DateTime hasta, TipoAgrupacionTiempo tipoAgrupacion)
        {
            var query = _ctx.Comanda
                .Where(c => c.RestauranteId == restauranteId
                         && c.HoraInicio >= desde
                         && c.HoraInicio <= hasta
                         && c.Pagos.Any());

            if (tipoAgrupacion == TipoAgrupacionTiempo.Hora)
            {
                var resultDb = await query
                    .GroupBy(c => c.HoraInicio.Hour)
                    .Select(g => new
                    {
                        Key = g.Key,
                        Total = g.Sum(c => c.Pagos.Sum(p => p.Total))
                    })
                    .OrderBy(x => x.Key)
                    .ToListAsync();

                return resultDb.Select(x => new VentaAgrupada
                {
                    Etiqueta = x.Key + "h",
                    Total = x.Total
                }).ToList();
            }
            else if (tipoAgrupacion == TipoAgrupacionTiempo.Dia)
            {
                var resultDb = await query
                    .GroupBy(c => c.HoraInicio.Date)
                    .Select(g => new
                    {
                        Key = g.Key,
                        Total = g.Sum(c => c.Pagos.Sum(p => p.Total))
                    })
                    .OrderBy(x => x.Key)
                    .ToListAsync();

                return resultDb.Select(x => new VentaAgrupada
                {
                    Etiqueta = x.Key.ToString("yyyy-MM-dd"),
                    Total = x.Total
                }).ToList();
            }
            else // Mes
            {
                var resultDb = await query
                    .GroupBy(c => new { c.HoraInicio.Year, c.HoraInicio.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Total = g.Sum(c => c.Pagos.Sum(p => p.Total))
                    })
                    .OrderBy(x => x.Year).ThenBy(x => x.Month)
                    .ToListAsync();

                return resultDb.Select(x => new VentaAgrupada
                {
                    Etiqueta = x.Year + "-" + x.Month.ToString("D2"),
                    Total = x.Total
                }).ToList();
            }
        }

        public async Task<List<EstadisticaMozoRaw>> ObtenerEstadisticasMozosRawAsync(int restauranteId, DateTime desde, DateTime hasta)
        {
            // 1. Obtener los mozos con sus mesas en una sola consulta
            var mozosData = await _ctx.Mozos
                .Include(m => m.IdEmpleadoNavigation)
                .Where(m => m.IdEmpleadoNavigation.RestauranteId == restauranteId)
                .Select(m => new
                {
                    m.IdEmpleado,
                    m.IdEmpleadoNavigation.Nombre,
                    MesaIds = m.Mesas.Select(mesa => mesa.Id).ToList()
                })
                .ToListAsync();

            // 2. Obtener todas las comandas y pagos del periodo en una sola consulta
            var comandasData = await _ctx.Comanda
                .Include(c => c.Pagos)
                .Include(c => c.EncuestaSatisfaccions)
                .Where(c => c.RestauranteId == restauranteId
                         && c.HoraInicio >= desde
                         && c.HoraInicio <= hasta)
                .Select(c => new
                {
                    c.MesaId,
                    c.HoraInicio,
                    c.HoraFin,
                    c.EstadoComandaId,
                    TotalPago = c.Pagos.Sum(p => p.Total),
                    PuntuacionesMozo = c.EncuestaSatisfaccions.Select(e => e.PuntuacionMozo).ToList()
                })
                .ToListAsync();

            // 3. Procesar en memoria (cero consultas N+1)
            var statsList = new List<EstadisticaMozoRaw>();

            foreach (var mozo in mozosData)
            {
                var mesasDelMozo = mozo.MesaIds;
                var comandasDelMozo = comandasData.Where(c => mesasDelMozo.Contains(c.MesaId)).ToList();

                int mesasAtendidas = comandasDelMozo.Select(c => c.MesaId).Distinct().Count();
                decimal facturacionTotal = comandasDelMozo.Sum(c => c.TotalPago);

                var comandasFinalizadas = comandasDelMozo.Where(c => c.HoraFin.HasValue).ToList();
                double? avgMinutes = null;
                if (comandasFinalizadas.Any())
                {
                    avgMinutes = comandasFinalizadas.Average(c => (c.HoraFin.Value - c.HoraInicio).TotalMinutes);
                }

                int comandasActivas = comandasDelMozo.Count(c => c.EstadoComandaId != (int)EstadoComanda.Finalizada
                                                              && c.EstadoComandaId != (int)EstadoComanda.Abierta);

                var todasLasPuntuaciones = comandasDelMozo.SelectMany(c => c.PuntuacionesMozo).ToList();
                double? avgEstrellas = todasLasPuntuaciones.Any() ? todasLasPuntuaciones.Average() : null;

                statsList.Add(new EstadisticaMozoRaw
                {
                    Nombre = mozo.Nombre,
                    MesasAtendidas = mesasAtendidas,
                    FacturacionTotal = facturacionTotal,
                    MinutosPromedioAtencion = avgMinutes,
                    ComandasActivas = comandasActivas,
                    PromedioEstrellas = avgEstrellas
                });
            }

            return statsList;
        }

        public async Task<List<IngredienteExcluidoStat>> ObtenerIngredientesExcluidosStatsAsync(int restauranteId, DateTime desde, DateTime hasta)
        {
            // 1. Obtener todas las exclusiones en el rango
            var exclusiones = await _ctx.ArticuloComandaIngredienteExcluidos
                .Where(ace => ace.ArticuloComanda.Comanda.RestauranteId == restauranteId
                           && ace.ArticuloComanda.Comanda.HoraInicio >= desde
                           && ace.ArticuloComanda.Comanda.HoraInicio <= hasta)
                .Select(ace => new
                {
                    ace.IngredienteId,
                    NombreIngrediente = ace.Ingrediente.IdInsumoNavigation.IdArticuloNavigation.Nombre,
                    PlatoId = ace.ArticuloComanda.ArticuloId,
                    NombrePlato = ace.ArticuloComanda.Articulo.Nombre
                })
                .ToListAsync();

            // 2. Obtener cantidad de pedidos por plato en el rango para calcular tasas
            var pedidosPorPlato = await _ctx.ArticuloComanda
                .Where(ac => ac.Comanda.RestauranteId == restauranteId
                          && ac.Comanda.HoraInicio >= desde
                          && ac.Comanda.HoraInicio <= hasta)
                .GroupBy(ac => ac.ArticuloId)
                .Select(g => new
                {
                    PlatoId = g.Key,
                    TotalPedidos = g.Sum(ac => ac.Cantidad)
                })
                .ToDictionaryAsync(x => x.PlatoId, x => x.TotalPedidos);

            // 3. Procesar en memoria
            var result = exclusiones
                .GroupBy(e => e.IngredienteId)
                .Select(g =>
                {
                    var count = g.Count();
                    var platoMasExcluidoGroup = g.GroupBy(x => new { x.PlatoId, x.NombrePlato })
                                                 .OrderByDescending(pg => pg.Count())
                                                 .FirstOrDefault();

                    var nombrePlato = platoMasExcluidoGroup?.Key.NombrePlato ?? string.Empty;
                    var idPlato = platoMasExcluidoGroup?.Key.PlatoId ?? 0;
                    var exclusionesPlato = platoMasExcluidoGroup?.Count() ?? 0;
                    var totalPedidosPlato = pedidosPorPlato.TryGetValue(idPlato, out var tp) ? tp : 0;

                    return new IngredienteExcluidoStat
                    {
                        IngredienteId = g.Key,
                        NombreIngrediente = g.First().NombreIngrediente,
                        CantidadExclusiones = count,
                        PlatoMasExcluido = nombrePlato,
                        ExclusionesEnPlatoMasExcluido = exclusionesPlato,
                        TotalPedidosPlatoMasExcluido = totalPedidosPlato
                    };
                })
                .OrderByDescending(x => x.CantidadExclusiones)
                .Take(5)
                .ToList();

            return result;
        }
    }
}
