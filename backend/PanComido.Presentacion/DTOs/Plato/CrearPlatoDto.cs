using System.ComponentModel.DataAnnotations;

namespace PanComido.Presentacion.DTOs.Plato
{
    public class CrearPlatoDto
    {
        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Required]
        public decimal PrecioVentaFinal { get; set; }

        [Required]
        public int TiempoPreparacionBase { get; set; }

        [Required]
        public int TipoPlatoId { get; set; }

        // Aunque el front se lo haya olvidado en el diseño, lo pedimos igual
        [Required]
        public int CategoriaPlatoId { get; set; }

        public string UrlImagen { get; set; }

        // El front nos manda solo los IDs de los botones que el usuario prendió (Vegano, Celíaco, etc.)
        public List<int> RestriccionesIds { get; set; } = new List<int>();

        // La lista de la tabla de la derecha (Ingredientes y cantidades)
        [Required]
        public List<IngredienteRecetaDto> Ingredientes { get; set; } = new List<IngredienteRecetaDto>();
    }



    public class IngredienteRecetaDto
    {
        [Required]
        public int InsumoId { get; set; }

        [Required]
        public decimal Cantidad { get; set; }

        public bool Opcional { get; set; }
    }

}
