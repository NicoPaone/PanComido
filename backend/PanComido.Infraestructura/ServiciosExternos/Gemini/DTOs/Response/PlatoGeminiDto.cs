using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.ServiciosExternos.Gemini.DTOs.Response
{
    public class PlatoGeminiDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int TiempoPreparacion { get; set; }
        public int PorcionesPosibles { get; set; }
        public List<IngredienteGeminiDto> IngredientesSugeridosIA { get; set; } = new();
    }
}
