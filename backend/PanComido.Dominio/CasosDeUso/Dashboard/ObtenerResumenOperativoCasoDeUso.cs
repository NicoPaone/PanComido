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
            DateTime hastaAjustado = hasta.Date.AddDays(1).AddTicks(-1);

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

            var totalesActuales = await _comandaRepositorio.ObtenerTotalesPeriodoAsync(restauranteId, desde, hastaAjustado);
            var ventasAgrupadas = await _comandaRepositorio.ObtenerVentasAgrupadasAsync(restauranteId, desde, hastaAjustado, tipoAgrupacion);

            TimeSpan duracionPeriodo = hastaAjustado - desde;
            DateTime desdeAnterior = desde.Subtract(duracionPeriodo);
            DateTime hastaAnterior = hastaAjustado.Subtract(duracionPeriodo);

            var totalesAnteriores = await _comandaRepositorio.ObtenerTotalesPeriodoAsync(restauranteId, desdeAnterior, hastaAnterior);

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
