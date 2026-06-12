using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.Dashboard
{
    public class ObtenerResumenOperativoCasoDeUso
    {
        private readonly IComandaRepositorio _comandaRepositorio;

        public ObtenerResumenOperativoCasoDeUso(IComandaRepositorio comandaRepositorio)
        {
            _comandaRepositorio = comandaRepositorio;
        }

        public async Task<ResumenOperativo> EjecutarAsync(int restauranteId, DateTime desde, DateTime hasta)
        {
            // 1. Ajustar 'hasta' al final del día si las fechas vienen sin hora
            DateTime hastaAjustado = hasta.Date.AddDays(1).AddTicks(-1);

            // 2. Determinar la agrupación para el gráfico
            TimeSpan diferencia = hastaAjustado - desde;
            string tipoAgrupacion = "Hora";
            if (diferencia.TotalDays > 35)
            {
                tipoAgrupacion = "Mes";
            }
            else if (diferencia.TotalDays > 1)
            {
                tipoAgrupacion = "Dia";
            }

            // 3. Obtener totales actuales y ventas agrupadas (Consultas SQL optimizadas)
            var totalesActuales = await _comandaRepositorio.ObtenerTotalesPeriodoAsync(restauranteId, desde, hastaAjustado);
            var ventasAgrupadas = await _comandaRepositorio.ObtenerVentasAgrupadasAsync(restauranteId, desde, hastaAjustado, tipoAgrupacion);

            // 4. Calcular el período anterior exacto para las variaciones porcentuales
            TimeSpan duracionPeriodo = hastaAjustado - desde;
            DateTime desdeAnterior = desde.Subtract(duracionPeriodo);
            DateTime hastaAnterior = hastaAjustado.Subtract(duracionPeriodo);

            var totalesAnteriores = await _comandaRepositorio.ObtenerTotalesPeriodoAsync(restauranteId, desdeAnterior, hastaAnterior);

            // 5. Cálculos de Variaciones y Promedios (Lógica de Negocio Pura)
            decimal ticketActual = totalesActuales.CantidadPedidos > 0 
                ? totalesActuales.TotalFacturado / totalesActuales.CantidadPedidos 
                : 0;

            decimal ticketAnterior = totalesAnteriores.CantidadPedidos > 0 
                ? totalesAnteriores.TotalFacturado / totalesAnteriores.CantidadPedidos 
                : 0;

            int diasPeriodo = (int)Math.Ceiling(diferencia.TotalDays);
            int promedioDiarioPedidos = diasPeriodo > 0 ? totalesActuales.CantidadPedidos / diasPeriodo : totalesActuales.CantidadPedidos;

            return new ResumenOperativo
            {
                TotalVentas = totalesActuales.TotalFacturado,
                TotalPedidos = totalesActuales.CantidadPedidos,
                TicketPromedio = ticketActual,
                PromedioDiarioPedidos = promedioDiarioPedidos,
                
                VariacionVentas = CalcularPorcentaje(totalesActuales.TotalFacturado, totalesAnteriores.TotalFacturado),
                VariacionPedidos = CalcularPorcentaje(totalesActuales.CantidadPedidos, totalesAnteriores.CantidadPedidos),
                VariacionTicket = CalcularPorcentaje(ticketActual, ticketAnterior),
                
                Grafico = ventasAgrupadas
            };
        }

        private decimal CalcularPorcentaje(decimal actual, decimal anterior)
        {
            if (anterior == 0) return actual > 0 ? 100m : 0m;
            return ((actual - anterior) / anterior) * 100m;
        }
    }
}
