using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.PagoCasoDeUso;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.DTOs.Pago;
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
        private readonly CrearPreferenciaMPCasoDeUso _crearPreferenciaMPCasoDeUso;
        private readonly PagoMapper _pagoMapper;

        public PagoController(
            SolicitarPagoEfectivoCasoDeUso solicitarPagoEfectivoCasoDeUso,
            ConfirmarPagoEfectivoCasoDeUso confirmarPagoEfectivoCasoDeUso,
            CrearPreferenciaMPCasoDeUso crearPreferenciaMPCasoDeUso,
            PagoMapper pagoMapper)
        {
            _solicitarPagoEfectivoCasoDeUso = solicitarPagoEfectivoCasoDeUso;
            _confirmarPagoEfectivoCasoDeUso = confirmarPagoEfectivoCasoDeUso;
            _crearPreferenciaMPCasoDeUso = crearPreferenciaMPCasoDeUso;
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

        //[HttpPost("solicitar-efectivo/{comandaId}/mozo")]
        //[Authorize(Roles = "Mozo, Gerente")]
        //public async Task<IActionResult> SolicitarPagoEfectivoMozo(int comandaId)
        //{
        //    // Extraemos el restauranteId de forma segura del token
        //    var restauranteId = HttpContext.ObtenerRestauranteId();

        //    var resultado = await _solicitarPagoEfectivoCasoDeUso.EjecutarAsync(comandaId, restauranteId);
        //    return Ok(resultado);
        //}

        [HttpPost("confirmar-pago-efectivo/{comandaId}")]
        [Authorize(Roles = "Mozo, Gerente")]
        public async Task<IActionResult> ConfirmarPagoEfectivo(int comandaId)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            var pagoConfirmado = await _confirmarPagoEfectivoCasoDeUso.EjecutarAsync(comandaId, restauranteId);
            var dto = _pagoMapper.aDto(pagoConfirmado);
            return Ok(dto);
        }

        [HttpPost("solicitar-mp/{comandaId}/comensal/{restauranteId}")]
        [AllowAnonymous]
        public async Task<IActionResult> SolicitarPagoMercadoPagoComensal(int restauranteId, int comandaId)
        {
            var initPoint = await _crearPreferenciaMPCasoDeUso.EjecutarAsync(comandaId, restauranteId);
            return Ok(new CrearPreferenciaResponseDto { InitPoint = initPoint });
        }
    }
}
