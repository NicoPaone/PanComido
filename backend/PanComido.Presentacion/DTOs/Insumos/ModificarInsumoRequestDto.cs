using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.Insumos
{
    public class ModificarInsumoRequestDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; }

        public string? Descripcion { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo.")]
        public decimal? PrecioVentaFinal { get; set; }

        public bool EsPrecioManual { get; set; }

        public bool EsVisibleEnCarta { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
        public decimal StockMinimo { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El stock recomendado no puede ser negativo.")]
        public decimal StockRecomendado { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida.")]
        public int CategoriaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una unidad de medida válida.")]
        public int UnidadDeMedidaId { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? Imagen { get; set; }
    }
}
