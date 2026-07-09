using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Presentacion.DTOs.Dashboard;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Mappers.Dashboard;
using PanComido.Presentacion.Sesion;
using System;
using System.Threading.Tasks;

namespace PanComido.Presentacion.Controllers
{
    [Route("gerente/dashboard/analisis-plato")]
    [ApiController]
    [Authorize(Roles = "Gerente")]
    public class DashboardPlatoAnalisisController : ControllerBase
    {
        private readonly ObtenerAnalisisPlatoCasoDeUso _obtenerAnalisisPlatoCasoDeUso;
        private readonly AplicarDescuentoCasoDeUso _aplicarDescuentoCasoDeUso;
        private readonly AgendarRecordatorioCasoDeUso _agendarRecordatorioCasoDeUso;
        private readonly PlatoAnalisisMapper _mapper;

        public DashboardPlatoAnalisisController(
            ObtenerAnalisisPlatoCasoDeUso obtenerAnalisisPlatoCasoDeUso,
            AplicarDescuentoCasoDeUso aplicarDescuentoCasoDeUso,
            AgendarRecordatorioCasoDeUso agendarRecordatorioCasoDeUso,
            PlatoAnalisisMapper mapper)
        {
            _obtenerAnalisisPlatoCasoDeUso = obtenerAnalisisPlatoCasoDeUso;
            _aplicarDescuentoCasoDeUso = aplicarDescuentoCasoDeUso;
            _agendarRecordatorioCasoDeUso = agendarRecordatorioCasoDeUso;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PlatoAnalisisDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PlatoAnalisisDto>> Obtener([FromQuery] string nombre)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            var resultado = await _obtenerAnalisisPlatoCasoDeUso.EjecutarAsync(restauranteId, nombre);

            if (resultado == null)
            {
                return NotFound(ApiErrorResponseFactory.Crear(HttpContext, "No se encontró el plato especificado.", "not_found"));
            }

            return Ok(_mapper.ParaDto(resultado));
        }

        [HttpPost("aplicar-descuento")]
        [ProducesResponseType(typeof(AplicarDescuentoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AplicarDescuentoResponse>> AplicarDescuento([FromBody] AplicarDescuentoRequest request)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            try
            {
                var resultado = await _aplicarDescuentoCasoDeUso.EjecutarAsync(restauranteId, request.PlatoId, request.PorcentajeDescuento);

                if (resultado == null)
                {
                    return NotFound(ApiErrorResponseFactory.Crear(HttpContext, "Plato no encontrado.", "not_found"));
                }

                return Ok(_mapper.ParaDto(resultado));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiErrorResponseFactory.Crear(HttpContext, ex.Message, "bad_request"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiErrorResponseFactory.Crear(HttpContext, ex.Message, "business_rule_violation"));
            }
        }

        [HttpPost("agendar-recordatorio")]
        [ProducesResponseType(typeof(AgendarRecordatorioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AgendarRecordatorioResponse>> AgendarRecordatorio([FromBody] AgendarRecordatorioRequest request)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            var resultado = await _agendarRecordatorioCasoDeUso.EjecutarAsync(restauranteId, request.PlatoId, request.AccionSugerida);

            if (resultado == null)
            {
                return NotFound(ApiErrorResponseFactory.Crear(HttpContext, "Plato no encontrado.", "not_found"));
            }

            return Ok(_mapper.ParaDto(resultado));
        }
    }
}
