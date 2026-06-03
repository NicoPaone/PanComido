using Microsoft.AspNetCore.Mvc;
using PanComido.Dominio.CasosDeUso.PagoCasoDeUso;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.SesionMock;

namespace PanComido.Presentacion.Controllers
{
    [Route("pago")]
    [ApiController]
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

        [HttpPost("solicitar-efectivo/{comandaId}")]
        public async Task<IActionResult> SolicitarPagoEfectivo(int comandaId)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            var resultado = await _solicitarPagoEfectivoCasoDeUso.EjecutarAsync(comandaId, restauranteId);
            return Ok(resultado);
        }

        [HttpPost("confirmar-pago-efectivo/{comandaId}")]
        public async Task<IActionResult> ConfirmarPagoEfectivo(int comandaId)
        {
            var restauranteId = HttpContext.ObtenerRestauranteId();
            var pagoConfirmado = await _confirmarPagoEfectivoCasoDeUso.EjecutarAsync(comandaId, restauranteId);
            var dto = _pagoMapper.aDto(pagoConfirmado);
            return Ok(dto);
        }
    }
}
