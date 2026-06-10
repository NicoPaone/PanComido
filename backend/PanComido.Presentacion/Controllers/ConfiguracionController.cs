using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.MetodoDePago;
using PanComido.Presentacion.DTOs.Restaurante;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.SesionMock;

namespace PanComido.Presentacion.Controllers
{
    [Route("configuracion")]
    [ApiController]
    public class ConfiguracionController : ControllerBase
    {
        private readonly ObtenerMetodosDePagoCasoDeUso _obtenerMetodosDePagoCasoDeUso;
        private readonly ActualizarMetodosDePagoCasoDeUso _actualizarMetodosDePagoCasoDeUso;
        private readonly ObtenerDatosDelLocalCasoDeUso _obtenerDatosDelLocalCasoDeUso;
        private readonly ActualizarDatosDelLocalCasoDeUso _actualizarDatosDelLocalCasoDeUso;
        private readonly MetodoDePagoMapper _metodoDePagoMapper;
        private readonly RestauranteMapper _restauranteMapper;

        public ConfiguracionController(
            ObtenerMetodosDePagoCasoDeUso obtenerMetodosDePagoCasoDeUso,
            ActualizarMetodosDePagoCasoDeUso actualizarMetodosDePagoCasoDeUso,
            ObtenerDatosDelLocalCasoDeUso obtenerDatosDelLocalCasoDeUso,
            ActualizarDatosDelLocalCasoDeUso actualizarDatosDelLocalCasoDeUso,
            MetodoDePagoMapper metodoDePagoMapper,
            RestauranteMapper restauranteMapper)
        {
            _obtenerMetodosDePagoCasoDeUso = obtenerMetodosDePagoCasoDeUso;
            _actualizarMetodosDePagoCasoDeUso = actualizarMetodosDePagoCasoDeUso;
            _obtenerDatosDelLocalCasoDeUso = obtenerDatosDelLocalCasoDeUso;
            _actualizarDatosDelLocalCasoDeUso = actualizarDatosDelLocalCasoDeUso;
            _metodoDePagoMapper = metodoDePagoMapper;
            _restauranteMapper = restauranteMapper;
        }

        [HttpGet("datos-local")]
        public async Task<ActionResult<RestauranteResponseDto>> ObtenerDatosDelLocal()
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();

            var datosRestaurante = await _obtenerDatosDelLocalCasoDeUso.EjecutarAsync(restauranteId);

            var dto = _restauranteMapper.aDto(datosRestaurante);
            return Ok(dto);
        }

        [HttpPut("actualizar-datos")]
        public async Task<ActionResult> ActualizarDatosLocal([FromBody] RestauranteRequestDto restauranteRequestDto)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            Restaurante restauranteDatos = _restauranteMapper.aDominio(restauranteRequestDto);

            var restauranteActualizado = await _actualizarDatosDelLocalCasoDeUso.EjecutarAsync(restauranteId, restauranteDatos);
            var dto = _restauranteMapper.aDto(restauranteActualizado);
            return Ok(dto);
        }

        [HttpGet("metodos-pago")]
        public async Task<ActionResult<List<MetodoDePagoResponseDto>>> ObtenerMetodosDePago()
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();

            var metodosDePago = await _obtenerMetodosDePagoCasoDeUso.EjecutarAsync(restauranteId);

            var dtos = _metodoDePagoMapper.aListaDto(metodosDePago);
            return Ok(dtos);
        }

        [HttpPut("habilitar-metodos-pago")]
        public async Task<ActionResult<List<MetodoDePagoRequestDto>>> HabilitarMetodoDePago([FromBody] List<MetodoDePagoRequestDto> metodoDePagoRequestDto)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            List<MetodoDePago> metodosDePago = _metodoDePagoMapper.aListaDominio(metodoDePagoRequestDto);

            await _actualizarMetodosDePagoCasoDeUso.EjecutarAsync(restauranteId, metodosDePago);

            return Ok();
        }
    }
}
