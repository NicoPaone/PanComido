namespace PanComido.Presentacion.DTOs.Cliente
{
    public class ConfirmarPedidoClienteRequestDto
    {
        public List<ItemPedidoClienteRequestDto> Items { get; set; } = new List<ItemPedidoClienteRequestDto>();
    }
}
