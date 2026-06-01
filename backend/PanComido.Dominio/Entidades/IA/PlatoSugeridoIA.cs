using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades.IA
{
    public class PlatoSugeridoIA
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int TiempoPreparacion { get; set; }
        public int PorcionesPosibles { get; set; }
        public List<IngredienteSugeridoIA> IngredientesSugeridosIA { get; set; } = new List<IngredienteSugeridoIA>();
    }
}