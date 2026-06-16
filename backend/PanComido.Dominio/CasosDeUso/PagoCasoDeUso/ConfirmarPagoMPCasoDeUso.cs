using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios.MercadoPago;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.PagoCasoDeUso
{
    public class ConfirmarPagoMPCasoDeUso
    {
        private readonly IMercadoPagoServicio _mercadoPagoServicio;
        private readonly IPagoRepositorio _pagoRepositorio;

        public ConfirmarPagoMPCasoDeUso(IMercadoPagoServicio mercadoPagoServicio, IPagoRepositorio pagoRepositorio)
        {
            _mercadoPagoServicio = mercadoPagoServicio;
            _pagoRepositorio = pagoRepositorio;
        }

        public async Task<Pago> EjecutarAsync(int paymentId)
        {
            ResultadoPagoMP resultadoPagoMP = await _mercadoPagoServicio.ConsultarPagoAsync(paymentId);
            Pago pago = await _pagoRepositorio.ObtenerPagoPorExternalReferenceAsync(resultadoPagoMP.ExternalReference);
            if (resultadoPagoMP.Status == "Approved")
            {

            }
        }
    }
}
