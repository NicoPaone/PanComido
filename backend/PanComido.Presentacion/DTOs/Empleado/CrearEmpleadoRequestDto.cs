using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.Empleado
{
    public class CrearEmpleadoRequestDto
    {
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Contrasenia { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado es requerido.")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es requerido.")]
        public string Rol { get; set; } = string.Empty;

        public List<int> TurnosIds { get; set; } = new();
    }
}
