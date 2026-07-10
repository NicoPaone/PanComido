using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.Autenticacion
{
    public class EjecutarResetDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Mínimo 8 caracteres.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9])(?!.*['"";]|.*--).*$", 
            ErrorMessage = "Debe tener 1 mayúscula, 1 número, 1 especial y no contener caracteres inválidos.")]
        public string NuevaContrasenia { get; set; } = string.Empty;
    }
}
