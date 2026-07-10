using System.Collections.Generic;

namespace PanComido.Dominio.Entidades
{
    public class NuevoMiseAndPlace
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int UnidadMedidaId { get; set; }
        public int CategoriaId { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal StockRecomendado { get; set; }
        public int RestauranteId { get; set; }

        public List<IngredienteDeMiseAndPlace> Ingredientes { get; set; } = new List<IngredienteDeMiseAndPlace>();
    }

    public class IngredienteDeMiseAndPlace
    {
        public int IngredienteId { get; set; }
        public decimal Cantidad { get; set; }
    }
}
