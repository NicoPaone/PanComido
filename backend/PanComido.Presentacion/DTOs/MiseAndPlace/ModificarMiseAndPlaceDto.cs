using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.MiseAndPlace
{
    public class ModificarMiseAndPlaceDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required]
        public int UnidadMedidaId { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
        public decimal StockMinimo { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El stock recomendado no puede ser negativo.")]
        public decimal StockRecomendado { get; set; }

        [Required]
        public List<IngredienteDeMiseAndPlaceDto> Ingredientes { get; set; } = new List<IngredienteDeMiseAndPlaceDto>();
    }

    public class IngredienteDeMiseAndPlaceDto
    {
        [Required]
        public int IngredienteId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad del ingrediente debe ser mayor a 0")]
        public decimal Cantidad { get; set; }
    }
}
