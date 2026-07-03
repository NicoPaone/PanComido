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
        private readonly SolicitarPagoCasoDeUso _solicitarPagoCasoDeUso;
        private readonly ConfirmarPagoCasoDeUso _confirmarPagoCasoDeUso;
        private readonly CrearPreferenciaMPCasoDeUso _crearPreferenciaMPCasoDeUso;
        private readonly ConfirmarPagoMPCasoDeUso _confirmarPagoMPCasoDeUso;
        private readonly PagoMapper _pagoMapper;
        private readonly ILogger<PagoController> _logger;

        public PagoController(
            SolicitarPagoCasoDeUso solicitarPagoCasoDeUso,
            ConfirmarPagoCasoDeUso confirmarPagoCasoDeUso,
            CrearPreferenciaMPCasoDeUso crearPreferenciaMPCasoDeUso,
            ConfirmarPagoMPCasoDeUso confirmarPagoMPCasoDeUso,
            PagoMapper pagoMapper,
            ILogger<PagoController> logger)
        {
            _solicitarPagoCasoDeUso = solicitarPagoCasoDeUso;
            _confirmarPagoCasoDeUso = confirmarPagoCasoDeUso;
            _crearPreferenciaMPCasoDeUso = crearPreferenciaMPCasoDeUso;
            _confirmarPagoMPCasoDeUso = confirmarPagoMPCasoDeUso;
            _pagoMapper = pagoMapper;
            _logger = logger;
        }

        [HttpPost("solicitar-pago/{comandaId}/comensal/{restauranteId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SolicitarPagoComensal(int comandaId, int restauranteId, [FromBody] SolicitarPagoRequestDto solicitarPagoRequest)
        {
            var resultado = await _solicitarPagoCasoDeUso.EjecutarAsync(comandaId, restauranteId, solicitarPagoRequest.MetodoPago);
            return Ok(resultado);
        }

        [HttpPost("confirmar-pago/{comandaId}")]
        [Authorize(Roles = "Mozo, Gerente")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDto),
        StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmarPago(int comandaId, [FromBody] ConfirmarPagoRequestDto request)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            var pagoConfirmado = await _confirmarPagoCasoDeUso.EjecutarAsync(comandaId,
        restauranteId, request.MetodoPago);
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

            long paymentId = long.Parse(notificacion.Data.Id);
            try
            {
                await _confirmarPagoMPCasoDeUso.EjecutarAsync(paymentId);
                _logger.LogInformation("Webhook MP procesado. PaymentId: {PaymentId}", paymentId);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Pago ya confirmado, se ignora reintento. PaymentId: {PaymentId}", paymentId);
            }
            return Ok();
        }
    }
}
