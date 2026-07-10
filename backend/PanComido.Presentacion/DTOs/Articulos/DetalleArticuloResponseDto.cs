using PanComido.Presentacion.DTOs.Ingredientes;

namespace PanComido.Presentacion.DTOs.Articulos
{
    public class DetalleArticuloResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string UrlImagen { get; set; }

        // Exclusivo de platos
        public int? TiempoPreparacionBase { get; set; }
        public string CategoriaPlato { get; set; }
        public string TipoPlato { get; set; }
        public string CategoriaBebida { get; set; }
        public List<string> Restricciones { get; set; } = new List<string>(); 
        public List<IngredientePersonalizableDto> IngredientesOpcionales { get; set; } = new List<IngredientePersonalizableDto>();
    }
}
