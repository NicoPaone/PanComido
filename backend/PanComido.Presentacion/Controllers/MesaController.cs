using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Presentacion.DTOs.Mesas;
using PanComido.Presentacion.SesionMock;

namespace PanComido.Presentacion.Controllers
{
    [Route("mesa")]
    [ApiController]
    public class MesaController : ControllerBase
    {
        private readonly OcuparMesaCasoDeUso _ocuparMesaCasoDeUso;


        public MesaController(OcuparMesaCasoDeUso ocuparMesaCasoDeUso)
        {
            _ocuparMesaCasoDeUso = ocuparMesaCasoDeUso;
        }

        [HttpPost("{id}/ocupar")]
        public async Task<IActionResult> Ocupar(int id, [FromBody] OcuparMesaRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                int restauranteId = HttpContext.ObtenerRestauranteId();

                await _ocuparMesaCasoDeUso.EjecutarAsync(restauranteId, id, request.CantidadComensales.Value);

                return StatusCode(201, new { mensaje = "Mesa ocupada exitosamente. Podés empezar a pedir." });
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Error interno del servidor." });
            }
        }
    }
}
