using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso.Resultados;
using PanComido.Presentacion.DTOs;
using System;
using System.Threading.Tasks;

namespace PanComido.Presentacion.Controllers
{
    [ApiController]
    [Route("api/restaurantes/{restauranteId}/mesas/fila-virtual")]
    public class FilaVirtualController : ControllerBase
    {
        private readonly AnotarseEnFilaMesaCasoDeUso _anotarseCasoDeUso;
        private readonly ObtenerEstadoFilaMesaCasoDeUso _obtenerEstadoCasoDeUso;
        private readonly CancelarTurnoFilaCasoDeUso _cancelarTurnoCasoDeUso;

        public FilaVirtualController(
            AnotarseEnFilaMesaCasoDeUso anotarseCasoDeUso, 
            ObtenerEstadoFilaMesaCasoDeUso obtenerEstadoCasoDeUso,
            CancelarTurnoFilaCasoDeUso cancelarTurnoCasoDeUso)
        {
            _anotarseCasoDeUso = anotarseCasoDeUso;
            _obtenerEstadoCasoDeUso = obtenerEstadoCasoDeUso;
            _cancelarTurnoCasoDeUso = cancelarTurnoCasoDeUso;
        }

        [HttpPost("anotarse")]
        public async Task<IActionResult> Anotarse(int restauranteId, [FromBody] AnotarseFilaMesaRequest request)
        {
            var resultado = await _anotarseCasoDeUso.EjecutarAsync(restauranteId, request.CantComensales);
            return Ok(resultado);
        }

        [HttpGet("turnos/{turnoId}")]
        public async Task<IActionResult> ObtenerEstado(int turnoId)
        {
            var estado = await _obtenerEstadoCasoDeUso.EjecutarAsync(turnoId);
            return Ok(estado);
        }

        [HttpPut("turnos/{turnoId}/cancelar")]
        public async Task<IActionResult> CancelarTurno(int turnoId)
        {
            await _cancelarTurnoCasoDeUso.EjecutarAsync(turnoId);
            return Ok(new { message = "Turno cancelado exitosamente." });
        }
    }
}
