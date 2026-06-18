using PanComido.Dominio.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class Pago
    {
        public int PagoId { get; set; }
        public int ComandaId { get; set; }
        public int? CierreId { get; set; }
        public int MetodoPagoId { get; set; }
        public string MetodoPagoDescripcion { get; set; }
        public decimal Total { get; set; }
        public DateTime? HoraFin { get; set; }
        public EstadoPago EstadoPago { get; set; }
    }
}