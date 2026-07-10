using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class Articulo
    {
        public int Id { get; set; }
        public int RestauranteId { get; set; }
        public int? CartaId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal? PrecioVentaFinal { get; set; }
        public decimal? PrecioGanancia { get; set; }
        public decimal? PrecioPromocional { get; set; }
        public string UrlImagen { get; set; }
        public bool EsVisibleEnCarta { get; set; }
        public decimal CostoCalculado { get; set; }
        public bool EsPrecioManual { get; set; }

    }
}
