namespace PanComido.Presentacion.DTOs.Insumos
{
    public class DetalleInsumoResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal? PrecioVentaFinal { get; set; }
        public bool EsPrecioManual { get; set; }
        public bool EsVisibleEnCarta { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal StockRecomendado { get; set; }
        public int CategoriaId { get; set; }
        public int UnidadDeMedidaId { get; set; }
        public string? UrlImagen { get; set; }
        public string Tipo { get; set; }
    }
}
