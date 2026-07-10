using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class ResumenRendimientoComercial
    {
        public List<RendimientoPlato> MasVendidos { get; set; } = new();
        public List<RendimientoPlato> MenosVendidos { get; set; } = new();
    }
}
