using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.BebidaPreparada
{
    public class CrearBebidaPreparadaRequestDto
    {
        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Required]
        public decimal PrecioVentaFinal { get; set; }

        public bool EsPrecioManual { get; set; }

        public bool EsVisibleEnCarta { get; set; }

        [Required(ErrorMessage = "La bebida preparada debe tener al menos un insumo en su receta.")]
        [MinLength(1, ErrorMessage = "La bebida preparada debe tener al menos un insumo en su receta.")]
        public List<InsumoRecetaDto> Insumos { get; set; } = new List<InsumoRecetaDto>();
    }

    public class InsumoRecetaDto
    {
        [Required]
        public int InsumoId { get; set; }

        [Required]
        public decimal Cantidad { get; set; }
    }
}
