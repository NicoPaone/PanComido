using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.Mesas
{
    public class OcuparMesaRequestDto
    {
        [Required(ErrorMessage = "Debe indicar la cantidad de comensales.")]
        [Range(1, 30, ErrorMessage = "La cantidad de comensales debe ser entre 1 y 30.")]
        public int? CantidadComensales { get; set; }
    }
}
