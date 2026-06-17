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

        public async Task<Pago?> EjecutarAsync(long paymentId)
        {
            ResultadoPagoMP resultado = await _mercadoPagoServicio.ConsultarPagoAsync(paymentId);

            if (resultado.Status == "approved")
            {
               return await _pagoRepositorio.ConfirmarPagoAsync(resultado.ExternalReference);
            }
            return null;
        }
    }
}
