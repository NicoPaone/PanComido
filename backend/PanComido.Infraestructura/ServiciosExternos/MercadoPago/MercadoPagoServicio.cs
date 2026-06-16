using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
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
            };

            var client = new PreferenceClient();

            Preference preference = await client.CreateAsync(request);
            return preference.InitPoint;
        }
    }
}
