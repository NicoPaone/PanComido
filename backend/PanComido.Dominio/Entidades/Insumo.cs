using PanComido.Dominio.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PanComido.Dominio.Entidades
{
    public class Insumo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal? PrecioVentaFinal { get; set; }
        // de tabla insumo
        public decimal StockMinimo { get; set; }

        public decimal StockActual { get; set; }
        public DateOnly? Vencimiento { get; set; }
        // calculado por el Use Case(no está en la BD)
        public EstadoStock? EstadoStock { get; set; }
        // subtipo: Ingrediente o Bebida
        public TipoInsumo Tipo { get; set; }
        // datos del subtipo
        public string Categoria { get; set; }      // categoria_ingrediente o categoria_bebida
        public string UnidadMedida { get; set; }   // solo ingredientes

    }
}
