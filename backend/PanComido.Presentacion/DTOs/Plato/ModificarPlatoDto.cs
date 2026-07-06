using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.Plato
{
    public class ModificarPlatoDto
    {
        [Required(ErrorMessage = "El nombre del plato no puede estar vacío.")]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio de venta final debe ser mayor que cero.")]
        public decimal PrecioVentaFinal { get; set; }

        [Required]
        public int TiempoPreparacionBase { get; set; }
        public bool EsPrecioManual { get; set; }

        [Required]
        public int TipoPlatoId { get; set; }

        [Required]
        public int CategoriaPlatoId { get; set; }

        public string UrlImagen { get; set; }
        
        public bool EsVisibleEnCarta { get; set; }

        public List<int> RestriccionesIds { get; set; } = new List<int>();

        [Required(ErrorMessage = "El plato debe tener al menos un ingrediente.")]
        [MinLength(1, ErrorMessage = "El plato debe tener al menos un ingrediente.")]
        public List<IngredienteRecetaDto> Ingredientes { get; set; } = new List<IngredienteRecetaDto>();
    }
}
