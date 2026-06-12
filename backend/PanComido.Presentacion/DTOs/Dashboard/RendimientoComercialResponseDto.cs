using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Presentacion.DTOs.Dashboard
{
    public class RendimientoComercialResponseDto
    {
        public List<PlatoRendimientoDto> MasVendidos { get; set; } = new();
        public List<PlatoRendimientoDto> MenosVendidos { get; set; } = new();
    }
}
