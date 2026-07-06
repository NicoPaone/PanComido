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
            [FromQuery] string fechaInicio, 
            [FromQuery] string fechaFin)
        {
            if (!DateTime.TryParse(fechaInicio, out var desde) || !DateTime.TryParse(fechaFin, out var hasta))
            {
                return BadRequest(new ErrorResponseDto { Error = "Formato de fechas inválido (YYYY-MM-DD)." });
            }

            if (desde > hasta)
            {
                return BadRequest(new ErrorResponseDto { Error = "La fecha de inicio debe ser anterior a la fecha de fin." });
            }

            try
            {
                var restauranteId = HttpContext.ObtenerRestauranteId();
                var pdfBytes = await _dashboardPdfCasoDeUso.EjecutarAsync(restauranteId, desde, hasta);

                string nombreArchivo = $"reporte_ejecutivo_{desde:yyyyMMdd}_{hasta:yyyyMMdd}.pdf";
                return File(pdfBytes, "application/pdf", nombreArchivo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Error = $"Error al generar reporte: {ex.Message}" });
            }
        }

        [HttpGet("personal/pdf")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerReportePersonal()
        {
            try
            {
                var restauranteId = HttpContext.ObtenerRestauranteId();
                var pdfBytes = await _personalPdfCasoDeUso.EjecutarAsync(restauranteId);

                string nombreArchivo = $"reporte_personal_{DateTime.Now:yyyyMMdd}.pdf";
                return File(pdfBytes, "application/pdf", nombreArchivo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Error = $"Error al generar reporte: {ex.Message}" });
            }
        }

        [HttpGet("ventas/pdf")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerReporteVentas(
            [FromQuery] string fechaInicio, 
            [FromQuery] string fechaFin)
        {
            if (!DateTime.TryParse(fechaInicio, out var desde) || !DateTime.TryParse(fechaFin, out var hasta))
            {
                return BadRequest(new ErrorResponseDto { Error = "Formato de fechas inválido (YYYY-MM-DD)." });
            }

            if (desde > hasta)
            {
                return BadRequest(new ErrorResponseDto { Error = "La fecha de inicio debe ser anterior a la fecha de fin." });
            }

            try
            {
                var restauranteId = HttpContext.ObtenerRestauranteId();
                var pdfBytes = await _ventasPdfCasoDeUso.EjecutarAsync(restauranteId, desde, hasta);

                string nombreArchivo = $"reporte_ventas_{desde:yyyyMMdd}_{hasta:yyyyMMdd}.pdf";
                return File(pdfBytes, "application/pdf", nombreArchivo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Error = $"Error al generar reporte: {ex.Message}" });
            }
        }
    }
}
