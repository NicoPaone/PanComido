using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class PlatoIngrediente
    {
        public int InsumoId { get; set; }
        public bool Opcional { get; set; }
        public Insumo Insumo { get; set; }
        public decimal Cantidad { get; set; }
    }
}
