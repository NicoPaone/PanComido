using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.Pedidos
{
    public class RecibirPedidoRequestDto
    {
        [MinLength(1, ErrorMessage = "Debe incluir al menos un ítem recibido")]
        public List<RecibirPedidoItemDto> ItemsPedidoRecibido { get; set; }
    }

    public class RecibirPedidoItemDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "El insumoId debe ser válido")]
        public int InsumoId { get; set; }

        [Required(ErrorMessage = "El nombre del lote es obligatorio")]
        [MaxLength(100)]
        public string NombreLote { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La bodega debe ser válida")]
        public int BodegaId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
        public decimal Cantidad { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria")]
        public string FechaVencimiento { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio de compra debe ser mayor a cero")]
        public decimal PrecioCompra { get; set; }
    }
}