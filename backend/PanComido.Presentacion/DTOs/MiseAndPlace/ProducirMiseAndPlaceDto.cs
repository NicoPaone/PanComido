using System;
using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.MiseAndPlace
{
    public class ProducirMiseAndPlaceDto
    {
        [Required(ErrorMessage = "La cantidad a producir es obligatoria.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public decimal Cantidad { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        public DateOnly FechaVencimiento { get; set; }

        [Required(ErrorMessage = "La bodega es obligatoria.")]
        public int BodegaId { get; set; }
    }
}
