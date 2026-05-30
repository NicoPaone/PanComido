using PanComido.Dominio.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PanComido.Dominio.Entidades
{
    public class Insumo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal? PrecioVentaFinal { get; set; }
        public decimal StockMinimo { get; set; }
        public TipoInsumo Tipo { get; set; }
        public string Categoria { get; set; }      
        public string UnidadMedida { get; set; }
        public List<Lote> Lotes { get; set; } = new List<Lote>();

        // propiedades para hacer alta
        public int CategoriaId { get; set; }
        public int UnidadDeMedidaId { get; set; }

        public int RestauranteId { get; set; }

        // propiedades calculadas
        public DateOnly? Vencimiento { get; set; }
        public decimal StockActual { get; set; }
        public EstadoStock? EstadoStock { get; set; }

    }
}
