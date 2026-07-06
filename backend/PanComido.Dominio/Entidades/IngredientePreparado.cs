using PanComido.Dominio.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class IngredientePreparado
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string unidadMedida { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal StockActual { get; set; }
        public decimal StockRecomendado { get; set; }

        public DateOnly FechaVencimientoProxima { get; set; }
        public EstadoStock EstadoStock { get; set; }

    }
}
