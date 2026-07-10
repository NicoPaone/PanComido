using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class FilaVirtual
    {
        public int Id { get; set; }
        public int RestauranteId { get; set; }
        public bool Habilitada { get; set; }
        public int TiempoPromedioComidaMinutos { get; set; } = 40;
    }
}
