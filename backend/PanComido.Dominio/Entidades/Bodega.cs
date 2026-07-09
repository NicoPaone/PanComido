using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class Bodega
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int TipoBodegaId { get; set; }
        public string TipoBodega { get; set; }
        public List<Insumo> Insumos { get; set; } = new();
    }
}
