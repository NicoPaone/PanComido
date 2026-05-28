using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades.Enums
{
    public enum EstadoStock
    {
        Critico,   // stock_actual < stock_minimo
        Bajo,      // stock_actual < stock_minimo * 2
        Normal     // todo bien
    }
}
