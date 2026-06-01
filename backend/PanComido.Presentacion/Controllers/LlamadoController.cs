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
        public async Task<ActionResult<LlamarMozoRequestDto>> CrearLlamado([FromBody] LlamarMozoRequestDto request)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            await _llamarMozoCasoDeUSo.EjecutarAsync(request.MesaId, request.CategoriaLlamadoId, request.Descripcion);
            return StatusCode(201, new { mensaje = "Llamado creado correctamente." });
        }

        [HttpGet("ver-pendientes/{mozoId}")]
        public async Task<ActionResult<List<LlamadoResponseDto>>> ObtenerLlamadosPendientes(int mozoId)
        {
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
