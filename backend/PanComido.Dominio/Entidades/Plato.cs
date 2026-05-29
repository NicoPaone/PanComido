using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class Plato
    {
       public int Id { get; set; }
        public String Nombre { get; set; }
        public int TiempoPreparacionBase { get; set; }
        public int Cantidad { get; set; }
        public string ? Observaciones { get; set; }


    }
}
