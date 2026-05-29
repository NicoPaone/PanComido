namespace PanComido.Presentacion.DTOs
{
    public class CrearPedidoResponseDto
    {
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public string ProveedorNombre { get; set; }
        public string? ProveedorTelefono { get; set; }
        public string Fecha { get; set; }
        public string Estado { get; set; }
        public List<PedidoInsumoResponseDto> ItemsInsumo { get; set; } = new();
    }
}
