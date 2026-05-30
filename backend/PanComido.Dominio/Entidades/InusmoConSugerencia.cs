using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class InusmoConSugerencia
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string UnidadMedida { get; set; }
        public decimal StockActual { get; set; }
        public decimal CantidadSugerida { get; set; }
        public string EstadoStock { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioTotalSugerido { get; set; }
    }
}
