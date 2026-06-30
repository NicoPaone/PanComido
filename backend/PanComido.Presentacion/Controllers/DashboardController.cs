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
        private readonly ObtenerRendimientoComercialCasoDeUso _obtenerRendimientoCasoDeUso;
        private readonly ObtenerResumenOperativoCasoDeUso _obtenerResumenOperativoCasoDeUso;
        private readonly ObtenerIngredientesExcluidosStatsCasoDeUso _obtenerIngredientesExcluidosStatsCasoDeUso;
        private readonly ObtenerAnalisisPlatoCasoDeUso _obtenerAnalisisPlatoCasoDeUso;
        private readonly AplicarDescuentoCasoDeUso _aplicarDescuentoCasoDeUso;
        private readonly AgendarRecordatorioCasoDeUso _agendarRecordatorioCasoDeUso;
        private readonly ResolverNotificacionCasoDeUso _resolverNotificacionCasoDeUso;
        private readonly DashboardMapper _mapper;
        private readonly PlatoAnalisisMapper _platoAnalisisMapper;

        public DashboardController(
            ObtenerVencimientosYCriticidadDashboardCasoDeUso obtenerVencimientosCasoDeUso,
            ObtenerRendimientoComercialCasoDeUso obtenerRendimientoCasoDeUso,
            ObtenerResumenOperativoCasoDeUso obtenerResumenOperativoCasoDeUso,
            ObtenerIngredientesExcluidosStatsCasoDeUso obtenerIngredientesExcluidosStatsCasoDeUso,
            ObtenerAnalisisPlatoCasoDeUso obtenerAnalisisPlatoCasoDeUso,
            AplicarDescuentoCasoDeUso aplicarDescuentoCasoDeUso,
            AgendarRecordatorioCasoDeUso agendarRecordatorioCasoDeUso,
            ResolverNotificacionCasoDeUso resolverNotificacionCasoDeUso,
            DashboardMapper mapper,
            PlatoAnalisisMapper platoAnalisisMapper)
        {
            _obtenerVencimientosCasoDeUso = obtenerVencimientosCasoDeUso;
            _obtenerRendimientoCasoDeUso = obtenerRendimientoCasoDeUso;
            _obtenerResumenOperativoCasoDeUso = obtenerResumenOperativoCasoDeUso;
            _obtenerIngredientesExcluidosStatsCasoDeUso = obtenerIngredientesExcluidosStatsCasoDeUso;
            _obtenerAnalisisPlatoCasoDeUso = obtenerAnalisisPlatoCasoDeUso;
            _aplicarDescuentoCasoDeUso = aplicarDescuentoCasoDeUso;
            _agendarRecordatorioCasoDeUso = agendarRecordatorioCasoDeUso;
            _resolverNotificacionCasoDeUso = resolverNotificacionCasoDeUso;
            _mapper = mapper;
            _platoAnalisisMapper = platoAnalisisMapper;
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
            [FromQuery] DateTime desde, 
            [FromQuery] DateTime hasta)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            var resumen = await _obtenerRendimientoCasoDeUso.EjecutarAsync(restauranteId, desde, hasta);
            var respuestaDto = _mapper.aRendimientoComercialDto(resumen);
            return Ok(respuestaDto);
        }

        [HttpGet("resumen")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerResumenOperativo(
            [FromQuery] DateTime desde, 
            [FromQuery] DateTime hasta)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            var resumen = await _obtenerResumenOperativoCasoDeUso.EjecutarAsync(restauranteId, desde, hasta);
            var respuestaDto = Presentacion.Mappers.Dashboard.ResumenOperativoMapper.ParaDto(resumen);
            return Ok(respuestaDto);
        }

        [HttpGet("ingredientes-excluidos")]
        [ProducesResponseType(typeof(List<IngredienteExcluidoStatDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerIngredientesExcluidos(
            [FromQuery] DateTime desde,
            [FromQuery] DateTime hasta)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            var stats = await _obtenerIngredientesExcluidosStatsCasoDeUso.EjecutarAsync(restauranteId, desde, hasta);
            var respuestaDto = _mapper.aListaIngredientesExcluidosDto(stats);
            return Ok(respuestaDto);
        }

        [HttpGet("analisis-plato")]
        [ProducesResponseType(typeof(PlatoAnalisisDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerAnalisisPlato([FromQuery] string nombre)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            var resultado = await _obtenerAnalisisPlatoCasoDeUso.EjecutarAsync(restauranteId, nombre);
            if (resultado == null)
            {
                return NotFound("No se encontró el plato especificado.");
            }
            var dto = _platoAnalisisMapper.ParaDto(resultado);
            return Ok(dto);
        }

        [HttpPost("analisis-plato/aplicar-descuento")]
        [ProducesResponseType(typeof(AplicarDescuentoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AplicarDescuento([FromBody] AplicarDescuentoRequest request)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            var resultado = await _aplicarDescuentoCasoDeUso.EjecutarAsync(restauranteId, request.PlatoId, request.PorcentajeDescuento);
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
        public async Task<IActionResult> AgendarRecordatorio([FromBody] AgendarRecordatorioRequest request)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            var resultado = await _agendarRecordatorioCasoDeUso.EjecutarAsync(restauranteId, request.PlatoId, request.AccionSugerida);
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
        public async Task<IActionResult> ResolverNotificacion([FromRoute] int id)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            await _resolverNotificacionCasoDeUso.EjecutarAsync(restauranteId, id);
            return Ok();
        }
    }
}