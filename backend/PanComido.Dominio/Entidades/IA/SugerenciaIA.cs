using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades.IA
{
    public class SugerenciaIA
    {
        public DateTime FechaSugerencia { get; set; }
        public List<PlatoSugeridoIA> PlatosSugeridos { get; set; } = new List<PlatoSugeridoIA>();
        public List<PlatoAnalisisIa> PlatosAnalisis { get; set; } = new List<PlatoAnalisisIa>();
    }
}