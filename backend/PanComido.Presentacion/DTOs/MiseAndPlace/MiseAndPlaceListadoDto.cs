using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.DTOs.MiseAndPlace
{
    public class MiseAndPlaceListadoDto
    {
        public int LoteId { get; set; }
        public int ArticuloId { get; set; }
        public int MiseAndPlaceId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public DateOnly? FechaVencimiento { get; set; }
        public string UnidadMedida { get; set; }
        public string Categoria { get; set; }
        public string Bodega { get; set; }
        public List<RecetaItemDto> Receta { get; set; } = new List<RecetaItemDto>();
    }

    public class RecetaItemDto
    {
        public int IngredienteId { get; set; }
        public string NombreIngrediente { get; set; }
        public decimal Cantidad { get; set; }
        public string UnidadMedida { get; set; }
    }
}
