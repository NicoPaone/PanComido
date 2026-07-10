using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.Autenticacion
{
    public class SolicitarResetDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
