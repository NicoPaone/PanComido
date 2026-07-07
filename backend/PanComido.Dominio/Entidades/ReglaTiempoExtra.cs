using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class ReglaTiempoExtra
    {
        public int Id { get; set; }
        public int RestauranteId { get; set; }
        public int PorcentajeOcupacionHasta { get; set; }
        public int MinutosExtra { get; set; }
    }
}
