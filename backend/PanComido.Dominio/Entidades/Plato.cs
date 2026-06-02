using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class Plato : Articulo
    {
        public int TiempoPreparacionBase { get; set; }

        // PASADOS A CLASE ArticuloComanda N:N entre articulo y comanda, Comanda tiene una lista de ArticulosComanda
        //public int Cantidad { get; set; }
        //public bool Entregado { get; set; }
        //public string ? ObservacionesIngredientes { get; set; }
        //public string ? ObservacionesGenerales { get; set; }
        public bool Destacado { get; set; }
        public bool Sugerencia { get; set; }
        public int CategoriaPlatoId { get; set; }
        public int TipoPlatoId { get; set; }
        public List<PlatoIngrediente> Ingredientes { get; set; } = new List<PlatoIngrediente>();
        public List<Restriccion> Restricciones { get; set; } = new List<Restriccion>();

        // valores para la carta:
        public string Categoria { get; set; }
        

        public string TipoPlato { get; set; }

        // valores para la carta:
        public string Categoria { get; set; }
        public List<string> Restricciones { get; set; } = new List<string>();

        public string TipoPlato { get; set; }

    }
}
