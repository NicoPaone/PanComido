namespace PanComido.Presentacion.DTOs.Cliente
{
    public class ItemPedidoClienteResponseDto
    {
        public int ArticuloId { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public bool Entregado { get; set; }
        public decimal PrecioUnitario { get; set; } 
        public decimal Subtotal { get; set; }
        public string? ObservacionesGenerales { get; set; }
        public List<string> ObservacionesIngredientes { get; set; } = new List<string>();
        public string NombreComensal { get; set; }

    }
}
