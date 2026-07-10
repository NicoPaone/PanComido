using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.Dashboard
{
    public class ObtenerResumenOperativoCasoDeUso
    {
        private readonly IDashboardRepositorio _dashboardRepositorio;
        private readonly IPlatoAnalisisRepositorio _platoAnalisisRepositorio;

        public ObtenerResumenOperativoCasoDeUso(
            IDashboardRepositorio dashboardRepositorio,
            IPlatoAnalisisRepositorio platoAnalisisRepositorio)
        {
            _dashboardRepositorio = dashboardRepositorio;
            _platoAnalisisRepositorio = platoAnalisisRepositorio;
        }

        public async Task<ResumenOperativo> EjecutarAsync(int restauranteId, DateTime desde, DateTime hasta)
        {
            DateTime hastaAjustado = hasta.Date.AddDays(1).AddTicks(-1);
            TipoAgrupacionTiempo tipoAgrupacion = DeterminarTipoAgrupacion(desde, hastaAjustado);

            var (desdeAnterior, hastaAnterior) = CalcularPeriodoAnterior(desde, hastaAjustado);

            var totalesActuales = await _dashboardRepositorio.ObtenerTotalesPeriodoAsync(restauranteId, desde, hastaAjustado);
            var ventasAgrupadas = await _dashboardRepositorio.ObtenerVentasAgrupadasAsync(restauranteId, desde, hastaAjustado, tipoAgrupacion);
            var totalesAnteriores = await _dashboardRepositorio.ObtenerTotalesPeriodoAsync(restauranteId, desdeAnterior, hastaAnterior);
            var recordatoriosActivos = await _platoAnalisisRepositorio.ObtenerRecordatoriosActivosAsync(restauranteId);
            var estadisticasMozosRaw = await _dashboardRepositorio.ObtenerEstadisticasMozosRawAsync(restauranteId, desde, hastaAjustado);

            return EnsamblarResumenOperativo(
                totalesActuales,
                totalesAnteriores,
                ventasAgrupadas,
                desde,
                hastaAjustado,
                recordatoriosActivos,
                estadisticasMozosRaw);
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
            List<Notificacion> recordatoriosActivos,
            List<EstadisticaMozoRaw> mozosRaw)
        {
            decimal ticketActual = CalcularTicketPromedio(actuales);
            decimal ticketAnterior = CalcularTicketPromedio(anteriores);
            int promedioDiarioPedidos = CalcularPromedioDiarioPedidos(actuales.CantidadPedidos, desde, hasta);

            var listRecordatorios = new List<DashboardAccionItem>();

            foreach (var item in recordatoriosActivos)
            {
                var (titulo, detalle) = item.ObtenerEstructura();

                listRecordatorios.Add(new DashboardAccionItem
                {
                    Id = item.Id,
                    Titulo = titulo,
                    Detalle = detalle,
                    Destino = NotificacionDestino.Carta.ToString().ToLower(),
                    Tono = NotificacionTono.Info.ToString().ToLower(),
                    Impacto = "Reevaluar demanda",
                    Prioridad = 4
                });
            }

            var listMozos = new List<EstadisticaMozo>();
            foreach (var raw in mozosRaw)
            {
                string tiempoPromedioAtencion = raw.MinutosPromedioAtencion.HasValue 
                    ? $"{Math.Round(raw.MinutosPromedioAtencion.Value)}m" 
                    : "30m";

                string estado = "Baja carga";
                if (raw.ComandasActivas > 4)
                {
                    estado = "Sobrecargado";
                }
                else if (raw.ComandasActivas >= 2)
                {
                    estado = "Optimo";
                }

                listMozos.Add(new EstadisticaMozo
                {
                    Nombre = raw.Nombre,
                    MesasAtendidas = raw.MesasAtendidas,
                    FacturacionTotal = raw.FacturacionTotal,
                    TiempoPromedioAtencion = tiempoPromedioAtencion,
                    Estado = estado,
                    CalificacionPromedio = raw.PromedioEstrellas
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
                Recordatorios = listRecordatorios,
                Mozos = listMozos
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

