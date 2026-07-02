using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.DatosTransferencia
{
    public class DatosTransferenciaRequestDto
    {
        [Required(ErrorMessage = "El alias es requerido.")]
        public string Alias { get; set; }

        [StringLength(22, MinimumLength = 22, ErrorMessage = "El CBU debe tener 22 caracteres.")]
        public string? Cbu { get; set; }

        [Required(ErrorMessage = "El número de cuenta es requerido.")]
        public string NumeroCuenta { get; set; }

        [Required(ErrorMessage = "El titular de la cuenta es requerido.")]
        public string TitularCuenta { get; set; }
    }
}
