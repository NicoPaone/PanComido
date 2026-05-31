namespace PanComido.Presentacion.DTOs.Proveedores
{
    public class ProveedorResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? NumeroTelefonoWsp { get; set; }
        public string? FechaUltimoPedido { get; set; }
        public List<string> Categorias { get; set; } = new();
    }
}
