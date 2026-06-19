using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Presentacion.Mappers;
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
        public async Task<IActionResult> ObtenerVencimientos()
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();

            var insumosPorVencer = await _obtenerVencimientosCasoDeUso.EjecutarAsync(restauranteId);

            var respuestaDto = _mapper.aListaVencimientosDto(insumosPorVencer);

            return Ok(respuestaDto);
        }
        [HttpGet("rendimiento")]
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
        public async Task<IActionResult> ObtenerResumenOperativo(
            [FromServices] ObtenerResumenOperativoCasoDeUso obtenerResumenOperativoCasoDeUso,
            [FromQuery] DateTime desde, 
            [FromQuery] DateTime hasta)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();

            var resumen = await obtenerResumenOperativoCasoDeUso.EjecutarAsync(restauranteId, desde, hasta);

            var respuestaDto = PanComido.Presentacion.Mappers.Dashboard.ResumenOperativoMapper.ParaDto(resumen);

            return Ok(respuestaDto);
        }
    }
}