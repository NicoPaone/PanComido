using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.PagoCasoDeUso;
using PanComido.Presentacion.DTOs.ErrorResponse;
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
        private readonly ConfirmarPagoMPCasoDeUso _confirmarPagoMPCasoDeUso;
        private readonly PagoMapper _pagoMapper;

        private readonly ILogger<PagoController> _logger;
        public PagoController(
            SolicitarPagoEfectivoCasoDeUso solicitarPagoEfectivoCasoDeUso,
            ConfirmarPagoEfectivoCasoDeUso confirmarPagoEfectivoCasoDeUso,
            CrearPreferenciaMPCasoDeUso crearPreferenciaMPCasoDeUso,
            ConfirmarPagoMPCasoDeUso confirmarPagoMPCasoDeUso,
            PagoMapper pagoMapper,
            ILogger<PagoController> logger)
        {
            _solicitarPagoEfectivoCasoDeUso = solicitarPagoEfectivoCasoDeUso;
            _confirmarPagoEfectivoCasoDeUso = confirmarPagoEfectivoCasoDeUso;
            _crearPreferenciaMPCasoDeUso = crearPreferenciaMPCasoDeUso;
            _confirmarPagoMPCasoDeUso = confirmarPagoMPCasoDeUso;
            _pagoMapper = pagoMapper;
            _logger = logger;
        }

        [HttpPost("solicitar-efectivo/{comandaId}/comensal/{restauranteId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SolicitarPagoEfectivoComensal(int comandaId, int restauranteId)
        {
            var resultado = await _solicitarPagoEfectivoCasoDeUso.EjecutarAsync(comandaId, restauranteId);
            return Ok(resultado);
        }

        [HttpPost("confirmar-pago-efectivo/{comandaId}")]
        [Authorize(Roles = "Mozo, Gerente")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmarPagoEfectivo(int comandaId)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            var pagoConfirmado = await _confirmarPagoEfectivoCasoDeUso.EjecutarAsync(comandaId, restauranteId);
            var dto = _pagoMapper.aDto(pagoConfirmado);
            return Ok(dto);
        }

        [HttpPost("solicitar-mp/{comandaId}/comensal/{restauranteId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SolicitarPagoMercadoPagoComensal(int comandaId, int restauranteId)
        {
            _logger.LogInformation("Solicitud preferencia MP. Comanda: {ComandaId}, Restaurante: {RestauranteId}", comandaId, restauranteId);
            var initPoint = await _crearPreferenciaMPCasoDeUso.EjecutarAsync(comandaId, restauranteId);
            return Ok(new CrearPreferenciaResponseDto { InitPoint = initPoint });
        }

        [HttpPost("webhook/mercado-pago")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmarPagoMercadoPago([FromBody] MercadoPagoWebhookDto notificacion)
        {
            _logger.LogInformation("Webhook MP recibido. Tipo: {Type}", notificacion.Type);
            if (notificacion?.Type != "payment") return Ok();

            try
            {
                long paymentId = long.Parse(notificacion.Data.Id);
                await _confirmarPagoMPCasoDeUso.EjecutarAsync(paymentId);
                _logger.LogInformation("Webhook MP procesado. PaymentId: {PaymentId}", paymentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando webhook de MP. PaymentId del body: {Id}", notificacion.Data?.Id);

            }
            return Ok();
        }
    }
}
