using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.PagoCasoDeUso;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Sesion;

namespace PanComido.Presentacion.Controllers
{
    [Route("pago")]
    [ApiController]
    [Authorize]
    public class PagoController : ControllerBase
    {
        private readonly SolicitarPagoEfectivoCasoDeUso _solicitarPagoEfectivoCasoDeUso;
        private readonly ConfirmarPagoEfectivoCasoDeUso _confirmarPagoEfectivoCasoDeUso;
        private readonly PagoMapper _pagoMapper;

        public PagoController(
            SolicitarPagoEfectivoCasoDeUso solicitarPagoEfectivoCasoDeUso,
            ConfirmarPagoEfectivoCasoDeUso confirmarPagoEfectivoCasoDeUso,
            PagoMapper pagoMapper)
        {
            _solicitarPagoEfectivoCasoDeUso = solicitarPagoEfectivoCasoDeUso;
            _confirmarPagoEfectivoCasoDeUso = confirmarPagoEfectivoCasoDeUso;
            _pagoMapper = pagoMapper;
        }

        [HttpPost("solicitar-efectivo/{comandaId}/comensal/{restauranteId}")]
        [AllowAnonymous]
        public async Task<IActionResult> SolicitarPagoEfectivoComensal(int comandaId, int restauranteId)
        {
            // El restauranteId viene de la URL. El framework y el Global Handler se encargan de los errores.
            var resultado = await _solicitarPagoEfectivoCasoDeUso.EjecutarAsync(comandaId, restauranteId);
            return Ok(resultado);
        }

        [HttpPost("solicitar-efectivo/{comandaId}/mozo")]
        [Authorize(Roles = "Mozo, Gerente")]
        public async Task<IActionResult> SolicitarPagoEfectivoMozo(int comandaId)
        {
            // Extraemos el restauranteId de forma segura del token
            var restauranteId = HttpContext.ObtenerRestauranteId();

            var resultado = await _solicitarPagoEfectivoCasoDeUso.EjecutarAsync(comandaId, restauranteId);
            return Ok(resultado);
        }
    }
}
