using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.Plato
{
    public class IngredienteRecetaResponseDto
    {
        public int InsumoId { get; set; }

        public decimal Cantidad { get; set; }

        public bool Opcional { get; set; }

        public string? Nombre { get; set; }
    }
}
