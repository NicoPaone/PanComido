namespace PanComido.Presentacion.DTOs.Pedidos
{
    public class EnviarPedidoResponseDto
    {
        public PedidoResponseDto PedidoConfirmado { get; set; }
        public string LinkWpp { get; set; }
    }
}
