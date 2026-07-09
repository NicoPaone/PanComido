using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Presentacion.DTOs.Dashboard;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Mappers.Dashboard;
using PanComido.Presentacion.Sesion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Presentacion.Controllers
{
    [Route("gerente/dashboard")]
    [ApiController]
    [Authorize(Roles = "Gerente")]
    public class DashboardController : ControllerBase
    {
        private readonly ObtenerVencimientosYCriticidadDashboardCasoDeUso _obtenerVencimientosCasoDeUso;
        private readonly ObtenerRendimientoComercialCasoDeUso _obtenerRendimientoCasoDeUso;
        private readonly ObtenerResumenOperativoCasoDeUso _obtenerResumenOperativoCasoDeUso;
        private readonly ObtenerIngredientesExcluidosStatsCasoDeUso _obtenerIngredientesExcluidosStatsCasoDeUso;
        private readonly ObtenerResumenSatisfaccionCasoDeUso _obtenerResumenSatisfaccionCasoDeUso;
        private readonly DashboardMapper _mapper;

        public DashboardController(
            ObtenerVencimientosYCriticidadDashboardCasoDeUso obtenerVencimientosCasoDeUso,
            ObtenerRendimientoComercialCasoDeUso obtenerRendimientoCasoDeUso,
            ObtenerResumenOperativoCasoDeUso obtenerResumenOperativoCasoDeUso,
            ObtenerIngredientesExcluidosStatsCasoDeUso obtenerIngredientesExcluidosStatsCasoDeUso,
            ObtenerResumenSatisfaccionCasoDeUso obtenerResumenSatisfaccionCasoDeUso,
            DashboardMapper mapper)
        {
            _obtenerVencimientosCasoDeUso = obtenerVencimientosCasoDeUso;
            _obtenerRendimientoCasoDeUso = obtenerRendimientoCasoDeUso;
            _obtenerResumenOperativoCasoDeUso = obtenerResumenOperativoCasoDeUso;
            _obtenerIngredientesExcluidosStatsCasoDeUso = obtenerIngredientesExcluidosStatsCasoDeUso;
            _obtenerResumenSatisfaccionCasoDeUso = obtenerResumenSatisfaccionCasoDeUso;
            _mapper = mapper;
        }

        [HttpGet("vencimientos")]
        [ProducesResponseType(typeof(List<InsumoPorVencerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<InsumoPorVencerDto>>> ObtenerVencimientos()
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            var insumosPorVencer = await _obtenerVencimientosCasoDeUso.EjecutarAsync(restauranteId);
            var respuestaDto = _mapper.aListaVencimientosDto(insumosPorVencer);
            return Ok(respuestaDto);
        }

        [HttpGet("rendimiento")]
        [ProducesResponseType(typeof(RendimientoComercialResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RendimientoComercialResponseDto>> ObtenerRendimiento([FromQuery] RangoFechasDashboardRequestDto request)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            var resumen = await _obtenerRendimientoCasoDeUso.EjecutarAsync(restauranteId, request.Desde!.Value, request.Hasta!.Value);
            var respuestaDto = _mapper.aRendimientoComercialDto(resumen);
            return Ok(respuestaDto);
        }

        [HttpGet("resumen")]
        [ProducesResponseType(typeof(ResumenOperativoResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResumenOperativoResponseDto>> ObtenerResumenOperativo([FromQuery] RangoFechasDashboardRequestDto request)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            var resumen = await _obtenerResumenOperativoCasoDeUso.EjecutarAsync(restauranteId, request.Desde!.Value, request.Hasta!.Value);
            var respuestaDto = Presentacion.Mappers.Dashboard.ResumenOperativoMapper.ParaDto(resumen);
            return Ok(respuestaDto);
        }

        [HttpGet("ingredientes-excluidos")]
        [ProducesResponseType(typeof(List<IngredienteExcluidoStatDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<IngredienteExcluidoStatDto>>> ObtenerIngredientesExcluidos([FromQuery] RangoFechasDashboardRequestDto request)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            var stats = await _obtenerIngredientesExcluidosStatsCasoDeUso.EjecutarAsync(restauranteId, request.Desde!.Value, request.Hasta!.Value);
            var respuestaDto = _mapper.aListaIngredientesExcluidosDto(stats);
            return Ok(respuestaDto);
        }

        [HttpGet("satisfaccion")]
        [ProducesResponseType(typeof(SatisfaccionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SatisfaccionResponseDto>> ObtenerSatisfaccion([FromQuery] RangoFechasDashboardRequestDto request)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();

            var resumen = await _obtenerResumenSatisfaccionCasoDeUso.EjecutarAsync(restauranteId, request.Desde!.Value, request.Hasta!.Value);

            var dto = ResumenSatisfaccionMapper.AResponseDto(resumen);

            return Ok(dto);
        }
    }
}
