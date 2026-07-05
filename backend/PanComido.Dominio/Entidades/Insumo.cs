using PanComido.Dominio.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PanComido.Dominio.Entidades
{
    public class Insumo : Articulo
    {
        public decimal StockMinimo { get; set; }
        public TipoInsumo? Tipo { get; set; }
        public string Categoria { get; set; }      
        public string UnidadMedida { get; set; }
        public List<Lote> Lotes { get; set; } = new List<Lote>();

        // propiedades para hacer alta
        public int CategoriaId { get; set; }
        public int UnidadDeMedidaId { get; set; }

        // propiedades calculadas
        public DateOnly? Vencimiento { get; set; }
        public decimal StockActual { get; set; }
        public EstadoStock? EstadoStock { get; set; }

        public CriticidadVencimiento? CriticidadVencimiento { get; set; }

        // para crear la carta 
        public List <PedidoInsumo> PedidoInsumos { get; set; } = new List<PedidoInsumo>();

    }
}
