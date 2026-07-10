using PanComido.Dominio.Entidades.Enums;
using PanComido.Presentacion.DTOs.CierreCaja;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class CierreCajaMapper
    {
        public CierreCajaResponseDto aDto(
            DOM.Cierre cierre,
            string turnoLaboralNombre,
            int cantidadTotalDePagos,
            decimal totalRecaudado,
            List<DOM.ResumenMetodoPago> resumenPorMetodo)
        {
            return new CierreCajaResponseDto
            {
                Fecha = cierre.Fecha,
                TurnoLaboralId = cierre.TurnoLaboralId,
                TurnoLaboralNombre = turnoLaboralNombre,
                CantidadTotalDePagos = cantidadTotalDePagos,
                TotalRecaudado = totalRecaudado,
                Diferencia = cierre.Diferencia,
                Sobrante = cierre.Sobrante,
                DetallePagos = resumenPorMetodo.Select(r => new DetallePagoDto
                {
                    MetodoPagoId = (int)r.Metodo,
                    MetodoPagoNombre = NombreMetodoPago(r.Metodo),
                    CantidadPagos = r.CantidadPagos,
                    Total = r.Total
                }).ToList()
            };
        }

        private static string NombreMetodoPago(MetodoPago metodo) => metodo switch
        {
            MetodoPago.Efectivo => "Efectivo",
            MetodoPago.Tarjeta => "Tarjeta",
            MetodoPago.Transferencia => "Transferencia",
            MetodoPago.MercadoPago => "Mercado Pago",
            _ => metodo.ToString()
        };
    }
}
