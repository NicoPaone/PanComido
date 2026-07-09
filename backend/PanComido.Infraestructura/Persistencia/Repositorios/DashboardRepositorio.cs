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
            var mozosData = await _ctx.Mozos
                .Where(m => m.IdEmpleadoNavigation.RestauranteId == restauranteId)
                .Select(m => new
                {
                    m.IdEmpleado,
                    m.IdEmpleadoNavigation.Nombre,
                    MesaIds = m.Mesas.Select(mesa => mesa.Id).ToList()
                })
                .ToListAsync();

            var comandasPorMesa = await _ctx.Comanda
                .Where(c => c.RestauranteId == restauranteId
                         && c.HoraInicio >= desde
                         && c.HoraInicio <= hasta)
                .GroupBy(c => c.MesaId)
                .Select(g => new
                {
                    MesaId = g.Key,
                    TotalPago = g.SelectMany(c => c.Pagos).Sum(p => p.Total),
                    CantidadComandas = g.Count(),
                    CantidadFinalizadas = g.Count(c => c.HoraFin.HasValue),
                    ComandasActivas = g.Count(c => c.EstadoComandaId != (int)EstadoComanda.Finalizada
                                                && c.EstadoComandaId != (int)EstadoComanda.Abierta)
                })
                .ToDictionaryAsync(x => x.MesaId);

            var minutosFinalizadasPorMesa = (await _ctx.Comanda
                .Where(c => c.RestauranteId == restauranteId
                         && c.HoraInicio >= desde
                         && c.HoraInicio <= hasta
                         && c.HoraFin.HasValue)
                .Select(c => new
                {
                    c.MesaId,
                    c.HoraInicio,
                    HoraFin = c.HoraFin!.Value
                })
                .ToListAsync())
                .GroupBy(c => c.MesaId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(c => (c.HoraFin - c.HoraInicio).TotalMinutes));

            var encuestasPorMesa = await _ctx.EncuestaSatisfaccions
                .Where(e => e.Comanda.RestauranteId == restauranteId
                         && e.Fecha >= desde
                         && e.Fecha <= hasta)
                .GroupBy(e => e.Comanda.MesaId)
                .Select(g => new
                {
                    MesaId = g.Key,
                    Promedio = g.Average(e => (double?)e.PuntuacionMozo)
                })
                .ToDictionaryAsync(x => x.MesaId, x => x.Promedio);

            var statsList = new List<EstadisticaMozoRaw>();

            foreach (var mozo in mozosData)
            {
                var mesasDelMozo = mozo.MesaIds;
                var statsMesas = mesasDelMozo
                    .Where(comandasPorMesa.ContainsKey)
                    .Select(mesaId => comandasPorMesa[mesaId])
                    .ToList();

                int mesasAtendidas = statsMesas.Count;
                decimal facturacionTotal = statsMesas.Sum(c => c.TotalPago);
                int cantidadFinalizadas = statsMesas.Sum(c => c.CantidadFinalizadas);
                double? avgMinutes = cantidadFinalizadas > 0
                    ? mesasDelMozo
                        .Where(minutosFinalizadasPorMesa.ContainsKey)
                        .Sum(mesaId => minutosFinalizadasPorMesa[mesaId]) / cantidadFinalizadas
                    : null;

                int comandasActivas = statsMesas.Sum(c => c.ComandasActivas);

                var promediosEncuestas = mesasDelMozo
                    .Where(encuestasPorMesa.ContainsKey)
                    .Select(mesaId => encuestasPorMesa[mesaId])
                    .Where(promedio => promedio.HasValue)
                    .Select(promedio => promedio!.Value)
                    .ToList();
                double? avgEstrellas = promediosEncuestas.Any() ? promediosEncuestas.Average() : null;

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

        public async Task<List<PanComido.Dominio.Entidades.EncuestaSatisfaccion>> ObtenerEncuestasPorPeriodoAsync(int restauranteId, DateTime desde, DateTime hasta)
        {
            var encuestas = await _ctx.EncuestaSatisfaccions
                .Where(e => e.Comanda.RestauranteId == restauranteId
                         && e.Fecha >= desde
                         && e.Fecha <= hasta)
                .Select(e => new PanComido.Dominio.Entidades.EncuestaSatisfaccion
                {
                    Id = e.Id,
                    ComandaId = e.ComandaId,
                    PuntuacionLugar = e.PuntuacionLugar,
                    PuntuacionComida = e.PuntuacionComida,
                    PuntuacionMozo = e.PuntuacionMozo,
                    Fecha = e.Fecha
                })
                .ToListAsync();

            return encuestas;
        }

    }
}
