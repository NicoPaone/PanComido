using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.ServiciosExternos.Gemini.DTOs.Response
{
    public class GeminiResponseDto
    {
        public List<PlatoGeminiDto> PlatosSugeridos { get; set; } = new();
    }
}
