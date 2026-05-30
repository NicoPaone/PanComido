namespace PanComido.Presentacion.DTOs.Pedidos
{
    public class ConfirmarPedidoResponseDto
    {
        public PedidoResponseDto PedidoConfirmado { get; set; }
        public string LinkWpp { get; set; }
    }
}
