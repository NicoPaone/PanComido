using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Dominio.Constantes;
using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.DTOs.FamiliaTipografica;
using PanComido.Presentacion.DTOs.FilaVirtual;
using PanComido.Presentacion.DTOs.MetodoDePago;
using PanComido.Presentacion.DTOs.PorcetajesGanancia;
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
        private readonly ObtenerPorcentajesCasoDeUso _obtenerPorcentajesCasoDeUso;
        private readonly ActualizarPorcentajesCasoDeUso _actualizarPorcentajesCasoDeUso;
        private readonly ActualizarFilaVirtualCasoDeUso _actualizarFilaVirtualCasoDeUso;
        private readonly ListarFamiliasTipograficasCasoDeUso _listarFamiliasTipograficasCasoDeUso;
        private readonly MetodoDePagoMapper _metodoDePagoMapper;
        private readonly RestauranteMapper _restauranteMapper;
        private readonly TurnoLaboralMapper _turnoLaboralMapper;
        private readonly PorcentajesGananciaMapper _porcentajesGananciaMapper;
        private readonly FilaVirtualMapper _filaVirtualMapper;
        private readonly FamiliaTipograficaMapper _familiaTipograficaMapper;

        public ConfiguracionController(
            ObtenerMetodosDePagoCasoDeUso obtenerMetodosDePagoCasoDeUso,
            ActualizarMetodosDePagoCasoDeUso actualizarMetodosDePagoCasoDeUso,
            ObtenerDatosDelLocalCasoDeUso obtenerDatosDelLocalCasoDeUso,
            ActualizarDatosDelLocalCasoDeUso actualizarDatosDelLocalCasoDeUso,
            ObtenerTurnosLaboralesCasoDeUso obtenerTurnosLaboralesCasoDeUso,
            ActualizarTurnosLaboralesCasoDeUso actualizarTurnosLaboralesCasoDeUso,
            ObtenerFilaVirtualCasoDeUso obtenerFilaVirtualCasoDeUso,
            ActualizarFilaVirtualCasoDeUso actualizarFilaVirtualCasoDeUSo,
            ObtenerPorcentajesCasoDeUso obtenerPorcentajesCasoDeUso,
            ActualizarPorcentajesCasoDeUso actualizarPorcentajesCasoDeUSo,
            ListarFamiliasTipograficasCasoDeUso listarFamiliasTipograficasCasoDeUso,
            MetodoDePagoMapper metodoDePagoMapper,
            RestauranteMapper restauranteMapper,
            TurnoLaboralMapper turnoLaboralMapper,
            PorcentajesGananciaMapper porcentajesGananciaMapper,
            FilaVirtualMapper filaVirtualMapper,
            FamiliaTipograficaMapper familiaTipograficaMapper)
        {
            _obtenerMetodosDePagoCasoDeUso = obtenerMetodosDePagoCasoDeUso;
            _actualizarMetodosDePagoCasoDeUso = actualizarMetodosDePagoCasoDeUso;
            _obtenerDatosDelLocalCasoDeUso = obtenerDatosDelLocalCasoDeUso;
            _actualizarDatosDelLocalCasoDeUso = actualizarDatosDelLocalCasoDeUso;
            _obtenerTurnosLaboralesCasoDeUso = obtenerTurnosLaboralesCasoDeUso;
            _actualizarTurnosLaboralesCasoDeUso = actualizarTurnosLaboralesCasoDeUso;
            _obtenerPorcentajesCasoDeUso = obtenerPorcentajesCasoDeUso;
            _actualizarPorcentajesCasoDeUso = actualizarPorcentajesCasoDeUSo;
            _obtenerFilaVirtualCasoDeUso = obtenerFilaVirtualCasoDeUso;
            _actualizarFilaVirtualCasoDeUso = actualizarFilaVirtualCasoDeUSo;
            _listarFamiliasTipograficasCasoDeUso = listarFamiliasTipograficasCasoDeUso;
            _metodoDePagoMapper = metodoDePagoMapper;
            _restauranteMapper = restauranteMapper;
            _turnoLaboralMapper = turnoLaboralMapper;
            _porcentajesGananciaMapper = porcentajesGananciaMapper;
            _filaVirtualMapper = filaVirtualMapper;
            _familiaTipograficaMapper = familiaTipograficaMapper;
        }

        [HttpGet("familias-tipograficas")]
        [ProducesResponseType(typeof(List<FamiliaTipograficaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<FamiliaTipograficaResponseDto>>> ObtenerFamiliasTipograficas()
        {

            var familiasTipograficas = await _listarFamiliasTipograficasCasoDeUso.EjecutarAsync();

            var dto = _familiaTipograficaMapper.aListaDto(familiasTipograficas);
            return Ok(dto);
        }

        [HttpGet("datos-local")]
        [ProducesResponseType(typeof(List<RestauranteResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RestauranteResponseDto>> ObtenerDatosDelLocal()
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();

            var datosRestaurante = await _obtenerDatosDelLocalCasoDeUso.EjecutarAsync(restauranteId);

            var dto = _restauranteMapper.aDto(datosRestaurante);
            return Ok(dto);
        }

        [HttpPut("actualizar-datos")]
        [ProducesResponseType(typeof(RestauranteResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ActualizarDatosLocal([FromForm]
      RestauranteRequestDto restauranteRequestDto,
           IFormFile? imagen)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            Restaurante restauranteDatos = _restauranteMapper.aDominio(restauranteRequestDto);

            Stream? stream = imagen?.OpenReadStream();
            string? nombreArchivo = imagen?.FileName;

            var restauranteActualizado = await _actualizarDatosDelLocalCasoDeUso
               .EjecutarAsync(restauranteId,
                              restauranteDatos,
                              RutasCloudinary.SistemaLogos,
                              stream,
                              nombreArchivo
                              );

            var dto = _restauranteMapper.aDto(restauranteActualizado);
            return Ok(dto);
        }

        [HttpGet("metodos-pago")]
        [ProducesResponseType(typeof(List<MetodoDePagoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<MetodoDePagoResponseDto>>> ObtenerMetodosDePago()
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();

            var metodosDePago = await _obtenerMetodosDePagoCasoDeUso.EjecutarAsync(restauranteId);

            var dtos = _metodoDePagoMapper.aListaDto(metodosDePago);
            return Ok(dtos);
        }

        [HttpGet("metodos-pago/{restauranteId}/comensal")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<MetodoDePagoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<MetodoDePagoResponseDto>>> ObtenerMetodosDePagoParaComensal(int restauranteId)
        {
            var metodosDePago = await _obtenerMetodosDePagoCasoDeUso.EjecutarAsync(restauranteId);
            var dtos = _metodoDePagoMapper.aListaDto(metodosDePago);
            return Ok(dtos);
        }


        [HttpPut("habilitar-metodos-pago")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<MetodoDePagoRequestDto>>> HabilitarMetodoDePago([FromBody] List<MetodoDePagoRequestDto> metodoDePagoRequest)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            List<MetodoDePago> metodosDePago = _metodoDePagoMapper.aListaDominio(metodoDePagoRequest);

            await _actualizarMetodosDePagoCasoDeUso.EjecutarAsync(restauranteId, metodosDePago);

            return Ok();
        }

        [HttpGet("turno")]
        [ProducesResponseType(typeof(List<TurnoLaboralResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<TurnoLaboralResponseDto>>> ObtenerTurnosLaborales()
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();

            var turnosLaborales = await _obtenerTurnosLaboralesCasoDeUso.EjecutarAsync(restauranteId);

            var dtos = _turnoLaboralMapper.aListaDto(turnosLaborales);
            return Ok(dtos);
        }

        [HttpPut("actualizar-turno")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ActualizarTurnoLaboral([FromBody] List<TurnoLaboralRequestDto> turnosLaboralesRequest)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            List<TurnoLaboral> turnosLaborales = _turnoLaboralMapper.aListaDominio(turnosLaboralesRequest);

            await _actualizarTurnosLaboralesCasoDeUso.EjecutarAsync(restauranteId, turnosLaborales);

            return Ok();
        }

        [HttpGet("obtener-porcentajes")]
        [ProducesResponseType(typeof(List<PorcentajesGananciaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PorcentajesGananciaResponseDto>> ObtenerPorcentajesGanancia()
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();

            var porcentajesGanancia = await _obtenerPorcentajesCasoDeUso.EjecutarAsync(restauranteId);

            var dtos = _porcentajesGananciaMapper.aDto(porcentajesGanancia);
            return Ok(dtos);
        }

        [HttpPut("actualizar-porcentajes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ActualizarPorcentajesGanancia([FromBody] PorcentajesGananciaRequestDto porcentajesGananciaRequest)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            PorcentajesGanancia porcentajesGanancia = _porcentajesGananciaMapper.aDominio(porcentajesGananciaRequest);

            await _actualizarPorcentajesCasoDeUso.EjecutarAsync(restauranteId, porcentajesGanancia.Platos, porcentajesGanancia.Bebidas);

            return Ok();
        }

        [HttpGet("fila-virtual")]
        [ProducesResponseType(typeof(List<FilaVirtualResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FilaVirtualResponseDto>> ObtenerFilaVirual()
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();

            var filaVirtual = await _obtenerFilaVirtualCasoDeUso.EjecutarAsync(restauranteId);

            var dto = _filaVirtualMapper.aDto(filaVirtual);
            return Ok(dto);
        }

        [HttpPut("habilitar-fila-virtual")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> HabilitarFilaVirtual([FromBody] FilaVirtualRequestDto filaVirtualRequest)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            FilaVirtual filaVirtual = _filaVirtualMapper.aDominio(filaVirtualRequest);

            await _actualizarFilaVirtualCasoDeUso.EjecutarAsync(restauranteId, filaVirtual.Habilitada);

            return Ok();
        }
    }
}
