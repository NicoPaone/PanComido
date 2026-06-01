namespace PanComido.Presentacion.DTOs.Lotes
{
    public class LoteResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int InsumoId { get; set; }
        public decimal Cantidad { get; set; }
        public DateOnly FechaVencimiento { get; set; }
        public int BodegaId { get; set; }
    }
}
