using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.ServiciosExternos.Gemini.DTOs.Request
{
    public class GeminiRequestDto
    {
        [JsonPropertyName("contents")]
        public List<GeminiContenidoDto> Contenidos { get; set; } = new();
    }
}
