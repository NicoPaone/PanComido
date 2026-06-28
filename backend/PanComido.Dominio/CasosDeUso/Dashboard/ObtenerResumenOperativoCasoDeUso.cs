using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
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
            TipoAgrupacionTiempo tipoAgrupacion = DeterminarTipoAgrupacion(desde, hastaAjustado);

            var totalesActuales = await _comandaRepositorio.ObtenerTotalesPeriodoAsync(restauranteId, desde, hastaAjustado);
            var ventasAgrupadas = await _comandaRepositorio.ObtenerVentasAgrupadasAsync(restauranteId, desde, hastaAjustado, tipoAgrupacion);

            var (desdeAnterior, hastaAnterior) = CalcularPeriodoAnterior(desde, hastaAjustado);
            var totalesAnteriores = await _comandaRepositorio.ObtenerTotalesPeriodoAsync(restauranteId, desdeAnterior, hastaAnterior);

            var recordatoriosActivos = await _platoAnalisisRepositorio.ObtenerRecordatoriosActivosAsync(restauranteId);

            return EnsamblarResumenOperativo(totalesActuales, totalesAnteriores, ventasAgrupadas, desde, hastaAjustado, recordatoriosActivos);
        }

        private TipoAgrupacionTiempo DeterminarTipoAgrupacion(DateTime desde, DateTime hasta)
        {
            double dias = (hasta - desde).TotalDays;
            if (dias > 35) return TipoAgrupacionTiempo.Mes;
            if (dias > 1) return TipoAgrupacionTiempo.Dia;
            return TipoAgrupacionTiempo.Hora;
        }

        private (DateTime desdeAnterior, DateTime hastaAnterior) CalcularPeriodoAnterior(DateTime desde, DateTime hasta)
        {
            TimeSpan duracion = hasta - desde;
            return (desde.Subtract(duracion), hasta.Subtract(duracion));
        }

        private ResumenOperativo EnsamblarResumenOperativo(
            TotalesPeriodo actuales, 
            TotalesPeriodo anteriores, 
            List<VentaAgrupada> grafico, 
            DateTime desde, 
            DateTime hasta,
            List<Notificacion> recordatoriosActivos)
        {
            decimal ticketActual = CalcularTicketPromedio(actuales);
            decimal ticketAnterior = CalcularTicketPromedio(anteriores);
            int promedioDiarioPedidos = CalcularPromedioDiarioPedidos(actuales.CantidadPedidos, desde, hasta);

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
                TotalVentas = actuales.TotalFacturado,
                TotalPedidos = actuales.CantidadPedidos,
                TicketPromedio = ticketActual,
                PromedioDiarioPedidos = promedioDiarioPedidos,
                
                VariacionVentas = CalcularPorcentaje(actuales.TotalFacturado, anteriores.TotalFacturado),
                VariacionPedidos = CalcularPorcentaje(actuales.CantidadPedidos, anteriores.CantidadPedidos),
                VariacionTicket = CalcularPorcentaje(ticketActual, ticketAnterior),
                
                Grafico = grafico,
                Recordatorios = listRecordatorios
            };
        }

        private decimal CalcularTicketPromedio(TotalesPeriodo totales)
        {
            return totales.CantidadPedidos > 0 ? totales.TotalFacturado / totales.CantidadPedidos : 0;
        }

        private int CalcularPromedioDiarioPedidos(int totalPedidos, DateTime desde, DateTime hasta)
        {
            TimeSpan diferencia = hasta - desde;
            int diasPeriodo = (int)Math.Ceiling(diferencia.TotalDays);
            return diasPeriodo > 0 ? totalPedidos / diasPeriodo : totalPedidos;
        }

        private decimal CalcularPorcentaje(decimal actual, decimal anterior)
        {
            if (anterior == 0) return actual > 0 ? 100m : 0m;
            return ((actual - anterior) / anterior) * 100m;
        }
    }
}
