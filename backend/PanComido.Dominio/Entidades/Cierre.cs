using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class Cierre
    {
        public int CierreId { get; set; }
        public int RestauranteId { get; set; }
        public int TurnoLaboralId { get; set; }
        public decimal Diferencia { get; set; }
        public decimal Sobrante { get; set; }
        public decimal TotalEfectivo { get; set; }
        public decimal TotalTarjeta { get; set; }
        public decimal TotalTransferencia { get; set; }
        public decimal TotalMercadoPago { get; set; }
        public DateOnly Fecha { get; set; }
    }    
}
