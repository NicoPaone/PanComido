using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.ServiciosExternos.Gemini.DTOs.Response
{
    public class IngredienteGeminiDto
    {
        public int InsumoId { get; set; }
        public string Nombre { get; set; }
        public decimal Cantidad { get; set; }
    }
}
