using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.Bodegas
{
    public class GuardarBodegaRequestDto
    {
        [Required(ErrorMessage = "El nombre de la bodega es obligatorio.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El tipo de bodega es obligatorio.")]
        public int TipoBodegaId { get; set; }
    }

}
