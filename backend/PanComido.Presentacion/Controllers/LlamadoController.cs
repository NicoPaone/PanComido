using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.LlamadoMozoCasoDeUso;
using PanComido.Presentacion.DTOs.Comanda;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.DTOs.Llamado;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;

namespace PanComido.Presentacion.Controllers
{
    [Route("llamado")]
    [ApiController]
   [Authorize(Roles = "Mozo")]
    public class LlamadoController : ControllerBase
    {
        private readonly LlamarMozoCasoDeUso _llamarMozoCasoDeUSo;
        private readonly ListarLlamadosPendientesCasoDeUso _listarLlamadosPendientesCasoDeUso;
        private readonly ResolverLlamadoCasoDeUso _resolverLlamadoCasoDeUso;
        private readonly LlamadoMapper _llamadoMapper;

        public LlamadoController(
            LlamarMozoCasoDeUso llamarMozoCasoDeUSo,
            ListarLlamadosPendientesCasoDeUso listarLlamadosPendientesCasoDeUso,
            ResolverLlamadoCasoDeUso resolverLlamadoCasoDeUso,
            LlamadoMapper llamadoMapper)
        {
            _llamarMozoCasoDeUSo = llamarMozoCasoDeUSo;
            _listarLlamadosPendientesCasoDeUso = listarLlamadosPendientesCasoDeUso;
            _resolverLlamadoCasoDeUso = resolverLlamadoCasoDeUso;
            _llamadoMapper = llamadoMapper;
        }

        [HttpPost("generar-llamado")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<LlamadoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LlamadoResponseDto>> CrearLlamado([FromBody] LlamarMozoRequestDto request)
        {
           
            var llamadoGuardado = await _llamarMozoCasoDeUSo.EjecutarAsync(request.restauranteId, request.MesaId, request.CategoriaLlamadoId, request.Descripcion);

            var dto = _llamadoMapper.aDto(llamadoGuardado);
            return StatusCode(201, dto);
        }

        [HttpGet("ver-pendientes")]
        [ProducesResponseType(typeof(List<LlamadoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<LlamadoResponseDto>>> ObtenerLlamadosPendientes()
        {
            var mozoId = HttpContext.ObtenerEmpleadoId();
            var llamados = await _listarLlamadosPendientesCasoDeUso.EjecutarAsync(mozoId);

            var dtos = _llamadoMapper.aListaDto(llamados);
            return Ok(dtos);
        }

        [HttpPut("resolver/{llamadoId}")]
        [ProducesResponseType(typeof(List<LlamadoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<LlamadoResponseDto>>> ResolverLlamado(int llamadoId)
        {
             await _resolverLlamadoCasoDeUso.EjecutarAsync(llamadoId);
            return Ok(new { mensaje = "Llamado marcado como resuelto." });
        }
    }
}
