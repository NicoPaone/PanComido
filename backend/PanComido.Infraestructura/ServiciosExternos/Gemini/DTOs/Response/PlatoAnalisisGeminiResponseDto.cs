using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PanComido.Infraestructura.ServiciosExternos.Gemini.DTOs.Response
{
    public class PlatoAnalisisGeminiResponseDto
    {
        [JsonPropertyName("diagnostico")]
        public string Diagnostico { get; set; } = string.Empty;

        [JsonPropertyName("alerta")]
        public string Alerta { get; set; } = "moderada";

        [JsonPropertyName("sugerencias")]
        public List<PlatoSugerenciaGeminiDto> Sugerencias { get; set; } = new();
    }

    public class PlatoSugerenciaGeminiDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("tipo")]
        public string Tipo { get; set; } = string.Empty;

        [JsonPropertyName("accion")]
        public string Accion { get; set; } = string.Empty;

        [JsonPropertyName("impacto")]
        public string Impacto { get; set; } = string.Empty;

        [JsonPropertyName("dificultad")]
        public string Dificultad { get; set; } = "baja";

        [JsonPropertyName("esAplicable")]
        public bool EsAplicable { get; set; } = true;
    }
}
