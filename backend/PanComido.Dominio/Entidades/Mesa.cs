using PanComido.Dominio.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class Mesa
    {
        public int Id { get; set; }
        public EstadoMesa EstadoMesa { get; set; }
        public int Numero { get; set; }
        public int CantPersonasMax { get; set; }
        public int GrillaId { get; set; }
        public int DimensionMesaId { get; set; }
    }
}
