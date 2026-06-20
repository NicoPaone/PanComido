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
        public int TipoElemento { get; set; } = 1;
        public string? Color { get; set; }
        public string? TextoObjeto { get; set; }

        // Relación N:M con Mozos
        public virtual ICollection<MozoMesa> MozosAsignados { get; set; } = new List<MozoMesa>();
    }
}
