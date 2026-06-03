namespace PanComido.Presentacion.DTOs.Cliente
{
    public class ComandaClienteEstadoResponseDto
    {
        public int ComandaId { get; set; }
        public string EstadoUI { get; set; }
        public decimal TotalAPagar { get; set; }
        public List<ItemPedidoClienteResponseDto> Items { get; set; } = new List<ItemPedidoClienteResponseDto>();
    }
}
