using System;
using System.Collections.Generic;

namespace PanComido.Dominio.Entidades
{
    public class MiseAndPlaceListadoDominio
    {
        public int LoteId { get; set; }
        public int ArticuloId { get; set; }
        public int MiseAndPlaceId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public DateOnly? FechaVencimiento { get; set; }
        public string UnidadMedida { get; set; }
        public string Categoria { get; set; }
        public string Bodega { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal StockRecomendado { get; set; }
        public List<RecetaItemDominio> Receta { get; set; } = new List<RecetaItemDominio>();
    }

    public class RecetaItemDominio
    {
        public int IngredienteId { get; set; }
        public string NombreIngrediente { get; set; }
        public decimal Cantidad { get; set; }
        public string UnidadMedida { get; set; }
        public decimal CostoUnitario { get; set; }
    }
}
