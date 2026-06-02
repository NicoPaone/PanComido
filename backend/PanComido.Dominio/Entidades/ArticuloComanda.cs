using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class ArticuloComanda
    {
        public int Id { get; set; } // El ID de la tabla articulo_comanda
        public int Cantidad { get; set; }
        public bool Entregado { get; set; }
        public string? ObservacionesIngredientes { get; set; }
        public string? ObservacionesGenerales { get; set; }

        public Articulo Articulo { get; set; }
    }
}
