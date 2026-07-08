using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PanComido.Dominio.ValueObjects;

namespace PanComido.Presentacion.DTOs.Empleado
{
    public class CrearEmpleadoRequestDto : IValidatableObject
    {
        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(120, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 120 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        [StringLength(160, ErrorMessage = "El email no puede superar los 160 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [MaxLength(128, ErrorMessage = "La contraseña no puede superar los 128 caracteres.")]
        public string Contrasenia { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado es requerido.")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es requerido.")]
        public string Rol { get; set; } = string.Empty;

        public List<int> TurnosIds { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!EstadoEmpleado.EsValido(Estado))
            {
                yield return new ValidationResult("El estado del empleado no es válido.", new[] { nameof(Estado) });
            }

            if (!RolEmpleado.EsValido(Rol))
            {
                yield return new ValidationResult("El rol del empleado no es válido.", new[] { nameof(Rol) });
            }
        }
    }
}
