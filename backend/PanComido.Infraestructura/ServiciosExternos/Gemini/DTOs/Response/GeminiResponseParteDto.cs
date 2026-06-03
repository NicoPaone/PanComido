using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.ServiciosExternos.Gemini.DTOs.Response
{
    public class GeminiResponseParteDto
    {
        [JsonPropertyName("text")]
        public string Texto { get; set; }
    }
}
