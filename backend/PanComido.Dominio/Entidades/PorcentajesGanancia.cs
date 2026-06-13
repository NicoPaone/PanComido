using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class PorcentajesGanancia
    {
        public List<PorcentajesCategoria> Platos { get; set; }
        public List<PorcentajesCategoria> Bebidas { get; set; }
    }
}
