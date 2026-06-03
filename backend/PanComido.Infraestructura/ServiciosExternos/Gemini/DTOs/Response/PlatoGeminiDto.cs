using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.ServiciosExternos.Gemini.DTOs.Response
{
    public class PlatoGeminiDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("descripcion")]
        public string Descripcion { get; set; }

        [JsonPropertyName("tiempoPreparacion")]
        public int TiempoPreparacion { get; set; }

        [JsonPropertyName("porcionesPosibles")]
        public int PorcionesPosibles { get; set; }

        [JsonPropertyName("ingredientesSugeridosIA")]
        public List<IngredienteGeminiDto> IngredientesSugeridosIA { get; set; } = new();
    }
}
