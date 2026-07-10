using System.Text.Json.Serialization;

namespace PanComido.Infraestructura.ServiciosExternos.Gemini.DTOs.Request
{
    public class GeminiRequestDto
    {
        [JsonPropertyName("contents")]
        public List<GeminiContenidoDto> Contenidos { get; set; } = new();

        [JsonPropertyName("generationConfig")]
        public GeminiGenerationConfigDto? GenerationConfig { get; set; }
    }

    public class GeminiGenerationConfigDto
    {
        [JsonPropertyName("thinkingConfig")]
        public GeminiThinkingConfigDto? ThinkingConfig { get; set; }
    }

    public class GeminiThinkingConfigDto
    {
        [JsonPropertyName("thinkingBudget")]
        public int ThinkingBudget { get; set; }
    }
}
