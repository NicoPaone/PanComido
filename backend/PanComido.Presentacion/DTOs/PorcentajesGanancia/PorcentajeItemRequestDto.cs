using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.PorcetajesGanancia
{
    public class PorcentajeItemRequestDto
    {
        [Required]
        public int Id { get; set; }

        [Range(0, 200, ErrorMessage = "El porcetaje debe ser mayor o igual a 0")]
        public decimal Porcentaje { get; set; }
    }
}
