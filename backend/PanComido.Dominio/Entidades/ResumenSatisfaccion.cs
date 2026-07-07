using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class ResumenSatisfaccion
    {
        public double PromedioComida { get; set; }
        public double PromedioLugar { get; set; }
        public double PromedioAtencion { get; set; }
        public int TotalEncuestas { get; set; }
        public int TotalDerivadosGoogleMaps { get; set; }
        public double PorcentajeDerivados { get; set; }
    }
}
