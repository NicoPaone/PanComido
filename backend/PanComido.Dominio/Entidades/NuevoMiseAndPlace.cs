using System;
using System.Collections.Generic;

namespace PanComido.Dominio.Entidades
{
    public class NuevoMiseAndPlace
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal RendimientoBase { get; set; }
        public DateOnly FechaVencimiento { get; set; }
        public int UnidadMedidaId { get; set; }
        public int CategoriaId { get; set; }
        public int BodegaId { get; set; }
        public int RestauranteId { get; set; }
        
        public List<IngredienteDeMiseAndPlace> Ingredientes { get; set; } = new List<IngredienteDeMiseAndPlace>();
    }

    public class IngredienteDeMiseAndPlace
    {
        public int IngredienteId { get; set; }
        public decimal Cantidad { get; set; }
    }
}
