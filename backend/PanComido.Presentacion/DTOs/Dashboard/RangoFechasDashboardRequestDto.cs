using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.Dashboard
{
    public class RangoFechasDashboardRequestDto : IValidatableObject
    {
        [Required(ErrorMessage = "La fecha de inicio es requerida.")]
        public DateTime? Desde { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida.")]
        public DateTime? Hasta { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Desde.HasValue && Hasta.HasValue && Desde > Hasta)
            {
                yield return new ValidationResult(
                    "La fecha de inicio debe ser anterior o igual a la fecha de fin.",
                    new[] { nameof(Desde), nameof(Hasta) });
            }
        }
    }
}
