using DOM = PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EF = PanComido.Infraestructura.Persistencia.Entidades;
using PanComido.Dominio.Entidades.Enums;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class PagoEntityMapper
    {
        public DOM.Pago paraDominio(EF.Pago efPago)
        {
            return new DOM.Pago
            {
                PagoId = efPago.Id,
                ComandaId = efPago.ComandaId,
                Total = efPago.Total,
                ExternalReference = efPago.ExternalReference,
                MetodoDePago = (MetodoPago)efPago.MetodoPagoId,
                EstadoPago = (EstadoPago)efPago.EstadoPagoId,
                CierreId = efPago.CierreId,
                //FechaHoraPago = efPago.FechaHoraPago
            };
        }

        public EF.Pago paraEntidad(DOM.Pago pagoDominio)
        {
            return new EF.Pago
            {
                ComandaId = pagoDominio.ComandaId,
                MetodoPagoId = (int)pagoDominio.MetodoDePago,
                EstadoPagoId = (int)pagoDominio.EstadoPago,
                Total = pagoDominio.Total,
                ExternalReference = pagoDominio.ExternalReference,
                CierreId = pagoDominio.CierreId
                //FechaHoraPago = pagoDominio.FechaHoraPago
            };
        }
    }
}
