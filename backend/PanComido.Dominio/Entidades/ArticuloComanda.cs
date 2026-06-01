using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class ArticuloComanda
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public bool Entregago { get; set; }
        public string? ObservacionesIngrediente { get; set; }
        public string? ObservacionesGenerales { get; set; }
    }
}
