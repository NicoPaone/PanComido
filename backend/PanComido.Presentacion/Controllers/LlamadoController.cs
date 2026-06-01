using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.MozoCasoDeUso;
using PanComido.Presentacion.DTOs.Llamado;
using PanComido.Presentacion.DTOs.Pedidos;
using PanComido.Presentacion.SesionMock;

namespace PanComido.Presentacion.Controllers
{
    [Route("llamado")]
    [ApiController]
    public class LlamadoController : ControllerBase
    {
        private readonly LlamarMozoCasoDeUso _llamarMozoCasoDeUSo;

        public LlamadoController(LlamarMozoCasoDeUso llamarMozoCasoDeUSo)
        {
            _llamarMozoCasoDeUSo = llamarMozoCasoDeUSo;
        }

        [HttpPost("generar-llamado")]
        public async Task<ActionResult<LlamarMozoRequestDto>> CrearLlamado([FromBody] LlamarMozoRequestDto request)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            await _llamarMozoCasoDeUSo.EjecutarAsync(request.MesaId, request.CategoriaLlamadoId, request.Descripcion);
            return StatusCode(201, new { mensaje = "Llamado creado correctamente." });
        }
    }
}
