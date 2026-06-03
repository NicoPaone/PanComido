using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.LlamadoMozoCasoDeUso;
using PanComido.Presentacion.DTOs.Llamado;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.SesionMock;

namespace PanComido.Presentacion.Controllers
{
    [Route("llamado")]
    [ApiController]
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
        public async Task<ActionResult<LlamadoResponseDto>> CrearLlamado([FromBody] LlamarMozoRequestDto request)
        {
            var restauranteId = HttpContext.ObtenerMozoId();
            var llamadoGuardado = await _llamarMozoCasoDeUSo.EjecutarAsync(restauranteId, 
                request.MesaId, request.CategoriaLlamadoId, request.Descripcion);

            var dto = _llamadoMapper.aDto(llamadoGuardado);
            return StatusCode(201, dto);
        }

        [HttpGet("ver-pendientes")]
        public async Task<ActionResult<List<LlamadoResponseDto>>> ObtenerLlamadosPendientes()
        {
            var mozoId = HttpContext.ObtenerMozoId();
            var llamados = await _listarLlamadosPendientesCasoDeUso.EjecutarAsync(mozoId);

            var dtos = _llamadoMapper.aListaDto(llamados);
            return Ok(dtos);
        }

        [HttpPut("resolver/{llamadoId}")]
        public async Task<ActionResult<List<LlamadoResponseDto>>> ResolverLlamado(int llamadoId)
        {
             await _resolverLlamadoCasoDeUso.EjecutarAsync(llamadoId);
            return Ok(new { mensaje = "Llamado marcado como resuelto." });
        }
    }
}
