using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.FilaVirtual;
using PanComido.Presentacion.DTOs.MetodoDePago;
using PanComido.Presentacion.DTOs.Restaurante;
using PanComido.Presentacion.DTOs.TurnoLaboral;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;

namespace PanComido.Presentacion.Controllers
{
    [Route("configuracion")]
    [ApiController]
    [Authorize(Roles = "Gerente")]
    public class ConfiguracionController : ControllerBase
    {
        private readonly ObtenerMetodosDePagoCasoDeUso _obtenerMetodosDePagoCasoDeUso;
        private readonly ActualizarMetodosDePagoCasoDeUso _actualizarMetodosDePagoCasoDeUso;
        private readonly ObtenerDatosDelLocalCasoDeUso _obtenerDatosDelLocalCasoDeUso;
        private readonly ActualizarDatosDelLocalCasoDeUso _actualizarDatosDelLocalCasoDeUso;
        private readonly ObtenerTurnosLaboralesCasoDeUso _obtenerTurnosLaboralesCasoDeUso;
        private readonly ActualizarTurnosLaboralesCasoDeUso _actualizarTurnosLaboralesCasoDeUso;
        private readonly ObtenerFilaVirtualCasoDeUso _obtenerFilaVirtualCasoDeUso;
        private readonly ActualizarFilaVirtualCasoDeUSo _actualizarFilaVirtualCasoDeUso;
        private readonly MetodoDePagoMapper _metodoDePagoMapper;
        private readonly RestauranteMapper _restauranteMapper;
        private readonly TurnoLaboralMapper _turnoLaboralMapper;
        private readonly FilaVirtualMapper _filaVirtualMapper;

        public ConfiguracionController(
            ObtenerMetodosDePagoCasoDeUso obtenerMetodosDePagoCasoDeUso,
            ActualizarMetodosDePagoCasoDeUso actualizarMetodosDePagoCasoDeUso,
            ObtenerDatosDelLocalCasoDeUso obtenerDatosDelLocalCasoDeUso,
            ActualizarDatosDelLocalCasoDeUso actualizarDatosDelLocalCasoDeUso,
            ObtenerTurnosLaboralesCasoDeUso obtenerTurnosLaboralesCasoDeUso,
            ActualizarTurnosLaboralesCasoDeUso actualizarTurnosLaboralesCasoDeUso,
            ObtenerFilaVirtualCasoDeUso obtenerFilaVirtualCasoDeUso,
            ActualizarFilaVirtualCasoDeUSo actualizarFilaVirtualCasoDeUSo,
            MetodoDePagoMapper metodoDePagoMapper,
            RestauranteMapper restauranteMapper,
            TurnoLaboralMapper turnoLaboralMapper,
            FilaVirtualMapper filaVirtualMapper)
        {
            _obtenerMetodosDePagoCasoDeUso = obtenerMetodosDePagoCasoDeUso;
            _actualizarMetodosDePagoCasoDeUso = actualizarMetodosDePagoCasoDeUso;
            _obtenerDatosDelLocalCasoDeUso = obtenerDatosDelLocalCasoDeUso;
            _actualizarDatosDelLocalCasoDeUso = actualizarDatosDelLocalCasoDeUso;
            _obtenerTurnosLaboralesCasoDeUso = obtenerTurnosLaboralesCasoDeUso;
            _actualizarTurnosLaboralesCasoDeUso = actualizarTurnosLaboralesCasoDeUso;
            _obtenerFilaVirtualCasoDeUso = obtenerFilaVirtualCasoDeUso;
            _actualizarFilaVirtualCasoDeUso = actualizarFilaVirtualCasoDeUSo;
            _metodoDePagoMapper = metodoDePagoMapper;
            _restauranteMapper = restauranteMapper;
            _turnoLaboralMapper = turnoLaboralMapper;
            _filaVirtualMapper = filaVirtualMapper;
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
        public async Task<ActionResult<List<MetodoDePagoRequestDto>>> HabilitarMetodoDePago([FromBody] List<MetodoDePagoRequestDto> metodoDePagoRequest)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            List<MetodoDePago> metodosDePago = _metodoDePagoMapper.aListaDominio(metodoDePagoRequest);

            await _actualizarMetodosDePagoCasoDeUso.EjecutarAsync(restauranteId, metodosDePago);

            return Ok();
        }

        [HttpGet("turno")]
        public async Task<ActionResult<List<TurnoLaboralResponseDto>>> ObtenerTurnosLaborales()
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();

            var turnosLaborales = await _obtenerTurnosLaboralesCasoDeUso.EjecutarAsync(restauranteId);

            var dtos = _turnoLaboralMapper.aListaDto(turnosLaborales);
            return Ok(dtos);
        }

        [HttpPut("actualizar-turno")]
        public async Task<ActionResult> ActualizarTurnoLaboral([FromBody] List<TurnoLaboralRequestDto> turnosLaboralesRequest)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            List<TurnoLaboral> turnosLaborales = _turnoLaboralMapper.aListaDominio(turnosLaboralesRequest);

            await _actualizarTurnosLaboralesCasoDeUso.EjecutarAsync(restauranteId, turnosLaborales);

            return Ok();
        }

        [HttpGet("fila-virtual")]
        public async Task<ActionResult<FilaVirtualResponseDto>> ObtenerFilaVirual()
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();

            var filaVirtual = await _obtenerFilaVirtualCasoDeUso.EjecutarAsync(restauranteId);

            var dto = _filaVirtualMapper.aDto(filaVirtual);
            return Ok(dto);
        }

        [HttpPut("habilitar-fila-virtual")]
        public async Task<ActionResult> HabilitarFilaVirtual([FromBody] FilaVirtualRequestDto filaVirtualRequest)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            FilaVirtual filaVirtual = _filaVirtualMapper.aDominio(filaVirtualRequest);

            await _actualizarFilaVirtualCasoDeUso.EjecutarAsync(restauranteId, filaVirtual.Habilitada);

            return Ok();
        }
    }
}
