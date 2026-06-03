namespace PanComido.Presentacion.DTOs
{
    public class ConfirmarPagoResponseDto
    {
        public int PagoId { get; set; }
        public int ComandaId { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; }
        public string HoraFin { get; set; }
    }
}
