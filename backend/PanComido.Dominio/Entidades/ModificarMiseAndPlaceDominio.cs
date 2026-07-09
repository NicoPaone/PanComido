using System;
using System.Collections.Generic;

namespace PanComido.Dominio.Entidades
{
    public class ModificarMiseAndPlaceDominio
    {
        public int LoteId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal RendimientoBase { get; set; }
        public DateOnly FechaVencimiento { get; set; }
        public int UnidadMedidaId { get; set; }
        public int CategoriaId { get; set; }
        public int BodegaId { get; set; }
        public List<IngredienteDeMiseAndPlace> Ingredientes { get; set; }
    }
}
