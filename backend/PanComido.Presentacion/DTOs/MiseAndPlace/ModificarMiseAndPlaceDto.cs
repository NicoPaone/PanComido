using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.MiseAndPlace
{
    public class ModificarMiseAndPlaceDto
    {
        [Required]
        public int LoteId { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Required]
        public decimal Cantidad { get; set; }

        [Required]
        public DateOnly FechaVencimiento { get; set; }

        [Required]
        public int UnidadMedidaId { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        [Required]
        public int BodegaId { get; set; }

        [Required]
        public List<IngredienteDeMiseAndPlaceDto> Ingredientes { get; set; }
    }

    public class IngredienteDeMiseAndPlaceDto
    {
        [Required]
        public int IngredienteId { get; set; }

        [Required]
        public decimal Cantidad { get; set; }
    }
}
