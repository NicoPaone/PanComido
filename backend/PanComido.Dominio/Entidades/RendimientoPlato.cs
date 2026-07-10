using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class RendimientoPlato
    {
        public int PlatoId { get; set; }
        public string Nombre { get; set; }
        public int UnidadesVendidas { get; set; }
        public decimal FacturacionTotal { get; set; }
    }
}
