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
        
        public string NombreComensal { get; set; }
        public int ArticuloId { get; set; }

        public int Cantidad { get; set; }
        public bool Entregado { get; set; }
        public string? ObservacionesGenerales { get; set; }
        public Articulo Articulo { get; set; }
        // para lectura
        public List<Articulo> IngredientesExcluidos { get; set; } = new List<Articulo>();
        // para escritura
        public List<int> IngredientesExcluidosIds { get; set; } = new List<int>();

    }
}
