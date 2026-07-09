using System;
using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.Lote
{
    public class CrearLoteDto
    {
        [Required(ErrorMessage = "El Insumo es obligatorio.")]
        public int InsumoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public decimal Cantidad { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        public DateOnly FechaVencimiento { get; set; }

        [Required(ErrorMessage = "La bodega es obligatoria.")]
        public int BodegaId { get; set; }
    }
}
