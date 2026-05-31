namespace PanComido.Presentacion.DTOs.Pedidos
{
    public class PedidoResponseDto
    {
        public int Id { get; set; }
        public string Fecha { get; set; }
        public string Estado { get; set; }
        public List<PedidoInsumoResponseDto> ItemsInsumo { get; set; } = new();
    }
}
