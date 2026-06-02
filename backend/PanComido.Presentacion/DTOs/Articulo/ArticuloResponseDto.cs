namespace PanComido.Presentacion.DTOs.Articulo
{
    public class ArticuloResponseDto
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
    }
}
