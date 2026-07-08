using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.BebidaPreparada
{
    public class ModificarBebidaPreparadaRequestDto
    {
        [Required(ErrorMessage = "El nombre de la bebida preparada no puede estar vacío.")]
        public string Nombre { get; set; }

        public string? Descripcion { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio de venta final debe ser mayor que cero.")]
        public decimal PrecioVentaFinal { get; set; }

        public bool EsPrecioManual { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? Imagen { get; set; }

        public bool EsVisibleEnCarta { get; set; }

        [Required(ErrorMessage = "La bebida preparada debe tener al menos un insumo en su receta.")]
        [MinLength(1, ErrorMessage = "La bebida preparada debe tener al menos un insumo en su receta.")]
        public List<InsumoRecetaDto> Insumos { get; set; } = new List<InsumoRecetaDto>();
    }
}
