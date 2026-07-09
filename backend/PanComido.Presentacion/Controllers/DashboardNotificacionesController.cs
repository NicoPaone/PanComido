using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Sesion;
using System.Threading.Tasks;

namespace PanComido.Presentacion.Controllers
{
    [Route("gerente/dashboard/notificaciones")]
    [ApiController]
    [Authorize(Roles = "Gerente")]
    public class DashboardNotificacionesController : ControllerBase
    {
        private readonly ResolverNotificacionCasoDeUso _resolverNotificacionCasoDeUso;

        public DashboardNotificacionesController(ResolverNotificacionCasoDeUso resolverNotificacionCasoDeUso)
        {
            _resolverNotificacionCasoDeUso = resolverNotificacionCasoDeUso;
        }

        [HttpPost("{id}/resolver")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Resolver([FromRoute] int id)
        {
            int restauranteId = HttpContext.ObtenerRestauranteId();
            await _resolverNotificacionCasoDeUso.EjecutarAsync(restauranteId, id);
            return Ok();
        }
    }
}
