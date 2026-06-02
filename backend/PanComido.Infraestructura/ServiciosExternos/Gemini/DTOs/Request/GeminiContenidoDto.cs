using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.ServiciosExternos.Gemini.DTOs.Request
{
    public class GeminiContenidoDto
    {
        public List<GeminiParteDto> Partes { get; set; } = new();
    }
}
