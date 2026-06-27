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
        private readonly IPlatoAnalisisRepositorio _platoAnalisisRepositorio;

        public ObtenerResumenOperativoCasoDeUso(
            IComandaRepositorio comandaRepositorio,
            IPlatoAnalisisRepositorio platoAnalisisRepositorio)
        {
            _comandaRepositorio = comandaRepositorio;
            _platoAnalisisRepositorio = platoAnalisisRepositorio;
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

            var recordatoriosActivos = await _platoAnalisisRepositorio.ObtenerRecordatoriosActivosAsync(restauranteId);
            var listRecordatorios = new List<DashboardAccionItem>();

            foreach (var item in recordatoriosActivos)
            {
                string desc = item.Descripcion;
                string titulo = desc;
                string detalle = "";
                
                int separatorIndex = desc.IndexOf(" - ");
                if (separatorIndex >= 0)
                {
                    titulo = desc.Substring(0, separatorIndex);
                    detalle = desc.Substring(separatorIndex + 3);
                }

                listRecordatorios.Add(new DashboardAccionItem
                {
                    Id = item.Id,
                    Titulo = titulo,
                    Detalle = detalle,
                    Destino = "carta",
                    Tono = "info",
                    Impacto = "Reevaluar demanda",
                    Prioridad = 4
                });
            }

            return new ResumenOperativo
            {
                TotalVentas = totalesActuales.TotalFacturado,
                TotalPedidos = totalesActuales.CantidadPedidos,
                TicketPromedio = ticketActual,
                PromedioDiarioPedidos = promedioDiarioPedidos,
                
                VariacionVentas = CalcularPorcentaje(totalesActuales.TotalFacturado, totalesAnteriores.TotalFacturado),
                VariacionPedidos = CalcularPorcentaje(totalesActuales.CantidadPedidos, totalesAnteriores.CantidadPedidos),
                VariacionTicket = CalcularPorcentaje(ticketActual, ticketAnterior),
                
                Grafico = ventasAgrupadas,
                Recordatorios = listRecordatorios
            };
        }

        private decimal CalcularPorcentaje(decimal actual, decimal anterior)
        {
            if (anterior == 0) return actual > 0 ? 100m : 0m;
            return ((actual - anterior) / anterior) * 100m;
        }
    }
}
