using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.ServiciosExternos.Gemini.DTOs.Response
{
    public class GeminiApiResponseDto
    {
        [JsonPropertyName("candidates")]
        public List<GeminiCandidatoDto> Candidatos { get; set; }
    }
}
