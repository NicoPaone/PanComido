using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.ReporteCasosDeUso;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Sesion;
using System;
using System.Threading.Tasks;

namespace PanComido.Presentacion.Controllers
{
    [Route("reporte")]
    [ApiController]
    [Authorize(Roles = "Gerente")]
    public class ReporteController : ControllerBase
    {
        private const int RangoMaximoDiasReporte = 366;

        private readonly GenerarReporteDashboardPdfCasoDeUso _dashboardPdfCasoDeUso;
        private readonly GenerarReportePersonalPdfCasoDeUso _personalPdfCasoDeUso;
        private readonly GenerarReporteVentasPdfCasoDeUso _ventasPdfCasoDeUso;

        public ReporteController(
            GenerarReporteDashboardPdfCasoDeUso dashboardPdfCasoDeUso,
            GenerarReportePersonalPdfCasoDeUso personalPdfCasoDeUso,
            GenerarReporteVentasPdfCasoDeUso ventasPdfCasoDeUso)
        {
            _dashboardPdfCasoDeUso = dashboardPdfCasoDeUso;
            _personalPdfCasoDeUso = personalPdfCasoDeUso;
            _ventasPdfCasoDeUso = ventasPdfCasoDeUso;
        }

        [HttpGet("dashboard/pdf")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerReporteDashboard(
            [FromQuery] DateOnly fechaInicio, 
            [FromQuery] DateOnly fechaFin)
        {
            var validacion = ValidarRango(fechaInicio, fechaFin);
            if (validacion != null)
            {
                return validacion;
            }

            var desde = fechaInicio.ToDateTime(TimeOnly.MinValue);
            var hasta = fechaFin.ToDateTime(TimeOnly.MinValue);
            var restauranteId = HttpContext.ObtenerRestauranteId();
            var pdfBytes = await _dashboardPdfCasoDeUso.EjecutarAsync(restauranteId, desde, hasta);

            string nombreArchivo = $"reporte_ejecutivo_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.pdf";
            return File(pdfBytes, "application/pdf", nombreArchivo);
        }

        [HttpGet("personal/pdf")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerReportePersonal()
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            var pdfBytes = await _personalPdfCasoDeUso.EjecutarAsync(restauranteId);

            string nombreArchivo = $"reporte_personal_{DateTime.Now:yyyyMMdd}.pdf";
            return File(pdfBytes, "application/pdf", nombreArchivo);
        }

        [HttpGet("ventas/pdf")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerReporteVentas(
            [FromQuery] DateOnly fechaInicio, 
            [FromQuery] DateOnly fechaFin)
        {
            var validacion = ValidarRango(fechaInicio, fechaFin);
            if (validacion != null)
            {
                return validacion;
            }

            var desde = fechaInicio.ToDateTime(TimeOnly.MinValue);
            var hasta = fechaFin.ToDateTime(TimeOnly.MinValue);
            var restauranteId = HttpContext.ObtenerRestauranteId();
            var pdfBytes = await _ventasPdfCasoDeUso.EjecutarAsync(restauranteId, desde, hasta);

            string nombreArchivo = $"reporte_ventas_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.pdf";
            return File(pdfBytes, "application/pdf", nombreArchivo);
        }

        private BadRequestObjectResult? ValidarRango(DateOnly fechaInicio, DateOnly fechaFin)
        {
            if (fechaInicio == default || fechaFin == default)
            {
                return BadRequest(CrearError("Las fechas son requeridas y deben usar formato YYYY-MM-DD.", "validation_error"));
            }

            if (fechaInicio > fechaFin)
            {
                return BadRequest(CrearError("La fecha de inicio debe ser anterior o igual a la fecha de fin.", "validation_error"));
            }

            int dias = fechaFin.DayNumber - fechaInicio.DayNumber + 1;
            if (dias > RangoMaximoDiasReporte)
            {
                return BadRequest(CrearError($"El rango máximo permitido para reportes es de {RangoMaximoDiasReporte} días.", "validation_error"));
            }

            return null;
        }

        private ErrorResponseDto CrearError(string mensaje, string codigo)
        {
            return new ErrorResponseDto
            {
                Error = mensaje,
                Code = codigo,
                TraceId = HttpContext.TraceIdentifier
            };
        }
    }
}
