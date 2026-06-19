using System.Collections.Generic;

namespace PanComido.Presentacion.DTOs.Plato
{
    public class DetallePlatoResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioVentaFinal { get; set; }
        public int TiempoPreparacionBase { get; set; }
        public int TipoPlatoId { get; set; }
        public int CategoriaPlatoId { get; set; }
        public string UrlImagen { get; set; }
        public bool EsVisibleEnCarta { get; set; }
        public List<int> RestriccionesIds { get; set; } = new List<int>();
        public List<IngredienteRecetaResponseDto> Ingredientes { get; set; } = new List<IngredienteRecetaResponseDto>();
    }
}
