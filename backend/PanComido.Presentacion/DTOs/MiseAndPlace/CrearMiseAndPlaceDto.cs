using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.MiseAndPlace
{
    public class CrearMiseAndPlaceDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal Cantidad { get; set; }

        [Required]
        public decimal RendimientoBase { get; set; }

        [Required]
        public DateOnly FechaVencimiento { get; set; }

        [Required]
        public int UnidadMedidaId { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        [Required]
        public int BodegaId { get; set; }

        [Required]
        public List<MiseAndPlaceIngredienteDto> Ingredientes { get; set; } = new List<MiseAndPlaceIngredienteDto>();
    }

    public class MiseAndPlaceIngredienteDto
    {
        [Required]
        public int IngredienteId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal Cantidad { get; set; }
    }
}
