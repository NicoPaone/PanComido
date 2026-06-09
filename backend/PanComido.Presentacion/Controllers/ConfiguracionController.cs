using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.MetodoDePago;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.SesionMock;

namespace PanComido.Presentacion.Controllers
{
    [Route("configacion")]
    [ApiController]
    public class ConfiguracionController : ControllerBase
    {
        private readonly ObtenerMetodosDePagoCasoDeUso _obtenerMetodosDePagoCasoDeUso;
        private readonly ActualizarMetodosDePagoCasoDeUso _actualizarMetodosDePagoCasoDeUso;
        private readonly MetodoDePagoMapper _metodoDePagoMapper;

        public ConfiguracionController(
            ObtenerMetodosDePagoCasoDeUso obtenerMetodosDePagoCasoDeUso,
            ActualizarMetodosDePagoCasoDeUso actualizarMetodosDePagoCasoDeUso,
            MetodoDePagoMapper metodoDePagoMapper)
        {
            _obtenerMetodosDePagoCasoDeUso = obtenerMetodosDePagoCasoDeUso;
            _actualizarMetodosDePagoCasoDeUso = actualizarMetodosDePagoCasoDeUso;
            _metodoDePagoMapper = metodoDePagoMapper;
        }

        [HttpGet("metodos-pago")]
        public async Task<ActionResult<List<MetodoDePagoResponseDto>>> ObtenerMetodosDePago()
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();

            var metodosDePago = await _obtenerMetodosDePagoCasoDeUso.EjecutarAsync(restauranteId);

            var dtos = _metodoDePagoMapper.aListaDto(metodosDePago);
            return Ok(dtos);
        }

        [HttpPut("metodos-pago")]
        public async Task<ActionResult<List<MetodoDePagoRequestDto>>> HabilitarMetodoDePago([FromBody] List<MetodoDePagoRequestDto> metodoDePagoRequestDto)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            List<MetodoDePago> metodosDePago = _metodoDePagoMapper.aListaDominio(metodoDePagoRequestDto);

            await _actualizarMetodosDePagoCasoDeUso.EjecutarAsync(restauranteId, metodosDePago);

            return Ok();
        }
    }
}
