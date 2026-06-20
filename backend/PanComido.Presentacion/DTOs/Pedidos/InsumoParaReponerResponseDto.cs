namespace PanComido.Presentacion.DTOs.Pedidos
{
    public class InsumoParaReponerResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string UnidadMedida { get; set; }
        public decimal StockActual { get; set; }
        public decimal CantidadSugerida { get; set; }
        public string EstadoStock { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioTotalSugerido { get; set; }
    }
}
