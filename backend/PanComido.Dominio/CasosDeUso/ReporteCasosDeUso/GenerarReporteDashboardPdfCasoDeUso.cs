using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.CasosDeUso.ReporteCasosDeUso
{
    public class GenerarReporteDashboardPdfCasoDeUso
    {
        private readonly ObtenerResumenOperativoCasoDeUso _resumenCasoDeUso;
        private readonly ObtenerRendimientoComercialCasoDeUso _rendimientoCasoDeUso;
        private readonly ObtenerVencimientosYCriticidadDashboardCasoDeUso _criticidadCasoDeUso;
        private readonly IPdfGeneradorServicio _pdfGenerador;

        public GenerarReporteDashboardPdfCasoDeUso(
            ObtenerResumenOperativoCasoDeUso resumenCasoDeUso,
            ObtenerRendimientoComercialCasoDeUso rendimientoCasoDeUso,
            ObtenerVencimientosYCriticidadDashboardCasoDeUso criticidadCasoDeUso,
            IPdfGeneradorServicio pdfGenerador)
        {
            _resumenCasoDeUso = resumenCasoDeUso;
            _rendimientoCasoDeUso = rendimientoCasoDeUso;
            _criticidadCasoDeUso = criticidadCasoDeUso;
            _pdfGenerador = pdfGenerador;
        }

        public async Task<byte[]> EjecutarAsync(int restauranteId, DateTime desde, DateTime hasta)
        {
            var resumen = await _resumenCasoDeUso.EjecutarAsync(restauranteId, desde, hasta);
            var rendimiento = await _rendimientoCasoDeUso.EjecutarAsync(restauranteId, desde, hasta);
            var criticidad = await _criticidadCasoDeUso.EjecutarAsync(restauranteId);

            return _pdfGenerador.GenerarReporteDashboard(resumen, rendimiento, criticidad, desde, hasta);
        }
    }
}
