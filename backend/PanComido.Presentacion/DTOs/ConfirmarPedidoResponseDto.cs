namespace PanComido.Presentacion.DTOs
{
    public class ConfirmarPedidoResponseDto
    {
        public PedidoResponseDto PedidoConfirmado { get; set; }
        public string LinkWpp { get; set; }
    }
}
