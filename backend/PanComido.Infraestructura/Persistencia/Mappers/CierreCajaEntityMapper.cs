using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class CierreCajaEntityMapper
    {
        public DOM.Cierre paraDominio(EF.Cierre efCierre)
        {
            return new DOM.Cierre
            {
                CierreId = efCierre.Id,
                RestauranteId = efCierre.RestauranteId,
                TurnoLaboralId = efCierre.TurnoLaboralId,
                Diferencia = efCierre.Diferencia,
                Sobrante = efCierre.Sobrante,
                TotalEfectivo = efCierre.TotalEfectivo,
                TotalTarjeta = efCierre.TotalTarjeta,
                TotalTransferencia = efCierre.TotalTransferencia,
                TotalMercadoPago = efCierre.TotalMercadoPago,
                Fecha = efCierre.Fecha
            };
        }

        public EF.Cierre paraEntidad(DOM.Cierre cierreDominio)
        {
            return new EF.Cierre
            {
                Id = cierreDominio.CierreId,
                RestauranteId = cierreDominio.RestauranteId,
                TurnoLaboralId = cierreDominio.TurnoLaboralId,
                Diferencia = cierreDominio.Diferencia,
                Sobrante = cierreDominio.Sobrante,
                TotalEfectivo = cierreDominio.TotalEfectivo,
                TotalTarjeta = cierreDominio.TotalTarjeta,
                TotalTransferencia = cierreDominio.TotalTransferencia,
                TotalMercadoPago = cierreDominio.TotalMercadoPago,
                Fecha = cierreDominio.Fecha
            };
        }
    }
}