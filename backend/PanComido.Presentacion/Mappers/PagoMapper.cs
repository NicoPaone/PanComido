using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.DTOs.Pedidos;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class PagoMapper
    {
        public ConfirmarPagoResponseDto aDto(DOM.Pago pago)
        {
            return new ConfirmarPagoResponseDto
            {
                PagoId = pago.PagoId, 
                ComandaId = pago.ComandaId,
                MetodoPago = pago.MetodoPagoDescripcion,
                Total = pago.Total,
                HoraFin = pago.HoraFin?.ToString("dd/MM/yyyy HH:mm")
            };
        }
    }
}