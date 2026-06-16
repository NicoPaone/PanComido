using PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Servicios.MercadoPago
{
    public interface IMercadoPagoServicio
    {
        Task<string> CrearPreferenciaAsync(string externalReference, decimal monto, string descripcion);

        Task<ResultadoPagoMP> ConsultarPagoAsync
            (long paymentId);
    }
}
