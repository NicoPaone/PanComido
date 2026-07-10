using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.Pedidos
{
    public class CrearPedidoRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "El pedido debe tener al menos un ítem")]
        public List<CrearPedidoItemDto> Items { get; set; } = new();
    }

    public class CrearPedidoItemDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "El insumoId debe ser válido")]
        public int InsumoId { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
        public decimal Cantidad { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "El precio de compra no puede ser negativo")]
        public decimal PrecioCompra { get; set; }
    }
}