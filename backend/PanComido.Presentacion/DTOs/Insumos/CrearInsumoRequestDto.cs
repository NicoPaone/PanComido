using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.Insumos
{
    public class CrearInsumoRequestDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo.")]
        public decimal? PrecioVentaFinal { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
        public decimal StockMinimo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida.")]
        public int CategoriaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una unidad de medida válida.")]
        public int UnidadDeMedidaId { get; set; }

        // datos para la creacion de lote inicial

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad inicial no puede ser negativa.")]
        public int CantidadInicial { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una bodega destino obligatoriamente.")]
        public int BodegaId { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        public DateOnly FechaVencimiento { get; set; }
    }
}
