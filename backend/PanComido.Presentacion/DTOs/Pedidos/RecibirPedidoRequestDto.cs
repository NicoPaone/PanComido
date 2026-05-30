namespace PanComido.Presentacion.DTOs.Pedidos
{
    public class RecibirPedidoRequestDto
    {
        public List<RecibirPedidoItemDto> ItemsPedidoRecibido { get; set; }
    }

    public class RecibirPedidoItemDto
    {
        public int InsumoId { get; set; }
        public string NombreLote { get; set; }
        public int BodegaId { get; set; }
        public decimal Cantidad { get; set; }
        public DateOnly FechaVencimiento { get; set; }
    }
}
