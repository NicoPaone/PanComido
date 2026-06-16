using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Servicios.MercadoPago;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.ServiciosExternos.MercadoPago
{
    public class MercadoPagoServicio : IMercadoPagoServicio
    {
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
                BackUrls = new PreferenceBackUrlsRequest
                {
                    //cambiar a nuestrar urls reales (que no sean localhost)
                    Success = "https://www.canva.com/design/DAHGUbn6JMw/n6vSal9_WcBau8Dr-y9ndg/edit",
                    Failure = "https://app.clickup.com/90171215989/v/b/6-901713590751-2",
                    Pending = "https://docs.google.com/document/d/1zVWsYOVtVLKrRgEQGIw9keGWLPDCA-ck959O6R-hsbI/edit?tab=t.iozh2a2tr3lv"
                },
                //Agregar cuando no sea local porque puede romper
               //AutoReturn = "approved"
            };

            var client = new PreferenceClient();

            Preference preference = await client.CreateAsync(request);
            return preference.InitPoint;
        }
    }
}
