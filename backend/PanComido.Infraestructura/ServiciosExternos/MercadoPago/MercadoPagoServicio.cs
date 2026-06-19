using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Options;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Servicios.MercadoPago;
using PanComido.Infraestructura.ServiciosExternos.Gemini;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.ServiciosExternos.MercadoPago
{
    public class MercadoPagoServicio : IMercadoPagoServicio
    {
        private readonly MercadoPagoConfiguracion _configuracionMP;

        public MercadoPagoServicio(IOptions<MercadoPagoConfiguracion> configuracionMP)
        {
            _configuracionMP = configuracionMP.Value;
        }

        public async Task<ResultadoPagoMP> ConsultarPagoAsync(long paymentId)
        {
            var client = new PaymentClient();

            var respuesta = await client.GetAsync(paymentId);
            ResultadoPagoMP resultadoPagoMp = new ResultadoPagoMP
            {
                Status = respuesta.Status,
                ExternalReference = respuesta.ExternalReference,
            };
            return resultadoPagoMp;
        }

        public async Task<string> CrearPreferenciaAsync(string externalReference, decimal monto, string descripcion)
        {
            var request = new PreferenceRequest
            {
                Items = new List<PreferenceItemRequest>
                {
                    new PreferenceItemRequest
                    {
                        Title = descripcion,
                        Quantity = 1,
                        UnitPrice = monto,
                    },
                },

                ExternalReference = externalReference,
                NotificationUrl = _configuracionMP.NotificationUrl,
                BackUrls = new PreferenceBackUrlsRequest
                {
                   //poner nuestrar urls reales (que no sean localhost)
                   Success = _configuracionMP.SuccessUrl,
                   Failure = _configuracionMP.FailureUrl,
                   Pending = _configuracionMP.PendingUrl,
                },
               AutoReturn = "approved"
            };

            var client = new PreferenceClient();

            Preference preference = await client.CreateAsync(request);
            return preference.InitPoint;
        }
    }
}
