using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Presentacion.DTOs.Dashboard;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Mappers.Dashboard;
using PanComido.Presentacion.Sesion;
using System;
using System.Threading.Tasks;

namespace PanComido.Presentacion.Controllers
{
    [Route("gerente/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ObtenerVencimientosYCriticidadDashboardCasoDeUso _obtenerVencimientosCasoDeUso;
        private readonly DashboardMapper _mapper;

        public DashboardController(
            ObtenerVencimientosYCriticidadDashboardCasoDeUso obtenerVencimientosCasoDeUso,
            DashboardMapper mapper)
        {
            _obtenerVencimientosCasoDeUso = obtenerVencimientosCasoDeUso;
            _mapper = mapper;
        }

        [HttpGet("vencimientos")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerVencimientos()
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();

            var insumosPorVencer = await _obtenerVencimientosCasoDeUso.EjecutarAsync(restauranteId);

            var respuestaDto = _mapper.aListaVencimientosDto(insumosPorVencer);

            return Ok(respuestaDto);
        }
        [HttpGet("rendimiento")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerRendimiento(
            [FromServices] ObtenerRendimientoComercialCasoDeUso obtenerRendimientoCasoDeUso,
            [FromQuery] DateTime desde, 
            [FromQuery] DateTime hasta)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();

            var resumen = await obtenerRendimientoCasoDeUso.EjecutarAsync(restauranteId, desde, hasta);

            var respuestaDto = _mapper.aRendimientoComercialDto(resumen);

            return Ok(respuestaDto);
        }

        [HttpGet("resumen")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerResumenOperativo(
            [FromServices] ObtenerResumenOperativoCasoDeUso obtenerResumenOperativoCasoDeUso,
            [FromQuery] DateTime desde, 
            [FromQuery] DateTime hasta)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();

            var resumen = await obtenerResumenOperativoCasoDeUso.EjecutarAsync(restauranteId, desde, hasta);

            var respuestaDto = Presentacion.Mappers.Dashboard.ResumenOperativoMapper.ParaDto(resumen);

            return Ok(respuestaDto);
        }

        [HttpGet("analisis-plato")]
        [ProducesResponseType(typeof(PlatoAnalisisDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerAnalisisPlato(
            [FromServices] ObtenerAnalisisPlatoCasoDeUso casoDeUso,
            [FromServices] PlatoAnalisisMapper mapper,
            [FromQuery] string nombre)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            var resultado = await casoDeUso.EjecutarAsync(restauranteId, nombre);
            if (resultado == null)
            {
                return NotFound("No se encontró el plato especificado.");
            }
            var dto = mapper.ParaDto(resultado);
            return Ok(dto);
        }

        [HttpPost("analisis-plato/aplicar-descuento")]
        [ProducesResponseType(typeof(AplicarDescuentoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AplicarDescuento(
            [FromServices] AplicarDescuentoCasoDeUso casoDeUso,
            [FromBody] AplicarDescuentoRequest request)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            var resultado = await casoDeUso.EjecutarAsync(restauranteId, request.PlatoId, request.PorcentajeDescuento);
            if (resultado == null)
            {
                return NotFound("Plato no encontrado.");
            }
            return Ok(new AplicarDescuentoResponse
            {
                Mensaje = resultado.Mensaje,
                PlatoId = resultado.PlatoId,
                PrecioNuevo = resultado.PrecioNuevo,
                Costo = resultado.Costo,
                MargenPctNuevo = resultado.MargenPctNuevo
            });
        }

        [HttpPost("analisis-plato/agendar-recordatorio")]
        [ProducesResponseType(typeof(AgendarRecordatorioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AgendarRecordatorio(
            [FromServices] AgendarRecordatorioCasoDeUso casoDeUso,
            [FromBody] AgendarRecordatorioRequest request)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            var resultado = await casoDeUso.EjecutarAsync(restauranteId, request.PlatoId, request.AccionSugerida);
            if (resultado == null)
            {
                return NotFound("Plato no encontrado.");
            }
            return Ok(new AgendarRecordatorioResponse
            {
                Mensaje = resultado.Mensaje,
                AccionItem = new DashboardAccionItemDto
                {
                    Titulo = resultado.Titulo,
                    Detalle = resultado.Detalle,
                    Destino = resultado.Destino,
                    Tono = resultado.Tono,
                    Impacto = resultado.Impacto,
                    Prioridad = resultado.Prioridad
                }
            });
        }

        [HttpPost("notificaciones/{id}/resolver")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResolverNotificacion(
            [FromServices] ResolverNotificacionCasoDeUso casoDeUso,
            [FromRoute] int id)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            await casoDeUso.EjecutarAsync(restauranteId, id);
            return Ok();
        }
    }
}