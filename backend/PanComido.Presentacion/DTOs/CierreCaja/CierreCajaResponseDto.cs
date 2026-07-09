namespace PanComido.Presentacion.DTOs.CierreCaja
{
    public class CierreCajaResponseDto
    {
        public DateOnly Fecha { get; set; }
        public int TurnoLaboralId { get; set; }
        public string TurnoLaboralNombre { get; set; }
        public int CantidadTotalDePagos { get; set; }
        public decimal TotalRecaudado { get; set; }
        public decimal Diferencia { get; set; }
        public decimal Sobrante { get; set; }
        public List<DetallePagoDto> DetallePagos { get; set; }
    }

    public class DetallePagoDto
    {
        public int MetodoPagoId { get; set; }
        public string MetodoPagoNombre { get; set; }
        public int CantidadPagos { get; set; }
        public decimal Total { get; set; }
    }
}
