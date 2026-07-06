namespace PanComido.Presentacion.DTOs.Carta
{
    public class ArticuloCartaResponseDto
    {
        public int ArticuloId { get; set; }
        public string Nombre { get; set; }
        public string UrlImagen { get; set; }
        public decimal PrecioVentaFinal { get; set; }
        public decimal Costo { get; set; }
        public bool VisibleEnCarta { get; set; }
        public bool Destacado { get; set; }
        public string TipoArticulo { get; set; }
        public string Categoria { get; set; }
        public int? CategoriaPlatoId { get; set; }

        public int? TiempoPreparacionBase { get; set; }
        public int? TiempoPreparacionEstimado { get; set; }

        public List<string> Restricciones { get; set; } = new();
        public bool EsPrecioManual { get; set; }
    }
}
