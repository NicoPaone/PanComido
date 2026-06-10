using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class Ubicacion
    {
        public int Id { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string CodigoPostal { get; set; }
    }
}
