namespace PanComido.Presentacion.DTOs.BebidaPreparada
{
    public class DetalleBebidaPreparadaResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioVentaFinal { get; set; }
        public string UrlImagen { get; set; }
        public bool EsPrecioManual { get; set; }
        public bool EsVisibleEnCarta { get; set; }
        public string Categoria { get; set; }
        public List<InsumoRecetaResponseDto> Insumos { get; set; } = new List<InsumoRecetaResponseDto>();
    }

    public class InsumoRecetaResponseDto
    {
        public int InsumoId { get; set; }
        public decimal Cantidad { get; set; }
        public string? Nombre { get; set; }
    }
}
