using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.ServiciosExternos.MercadoPago
{
    public class MercadoPagoConfiguracion
    {
      public string NotificationUrl { get; set; }
      public string SuccessUrl { get; set; }    
      public string FailureUrl { get; set; }    
      public string PendingUrl { get; set; }   

   }
}
