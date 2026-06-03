using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.ServiciosExternos.Gemini.DTOs.Response
{
    public class IngredienteGeminiDto
    {
        [JsonPropertyName("insumoId")]
        public int InsumoId { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("cantidad")]
        public decimal Cantidad { get; set; }
    }
}
