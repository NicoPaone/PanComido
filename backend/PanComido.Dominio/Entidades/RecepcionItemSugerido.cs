using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class RecepcionItemSugerido
    {
        public int InsumoId { get; set; }
        public string NombreInsumo { get; set; }
        public decimal Cantidad { get; set; }
        public string NombreLote { get; set; }
        public int BodegaIdSug { get; set; }
        public DateOnly FechaVencimientoSug { get; set; }
    }
}
