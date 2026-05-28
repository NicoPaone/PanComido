namespace PanComido.Presentacion.DTOs
{
    public class PedidoInsumoResponseDto
    {
        public int InsumoId { get; set; }
        public string NombreInsumo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioCompra { get; set; }
    }
}