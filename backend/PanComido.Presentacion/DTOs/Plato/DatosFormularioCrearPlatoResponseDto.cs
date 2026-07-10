using System.Collections.Generic;
using PanComido.Presentacion.DTOs.PorcetajesGanancia;

namespace PanComido.Presentacion.DTOs.Plato
{
    public class DatosFormularioCrearPlatoResponseDto
    {
        public List<ItemDesplegableDto> TiposPlato { get; set; } = new List<ItemDesplegableDto>();
        public List<ItemDesplegableDto> CategoriasPlato { get; set; } = new List<ItemDesplegableDto>();
        public List<ItemDesplegableDto> Restricciones { get; set; } = new List<ItemDesplegableDto>();

        // El frontend recibe una sola lista unificada para el buscador
        public List<IngredienteDisponibleDto> Ingredientes { get; set; } = new List<IngredienteDisponibleDto>();

        public PorcentajesGananciaResponseDto Porcentajes { get; set; }

    }

    public class ItemDesplegableDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }


    public class IngredienteDisponibleDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string UnidadMedida { get; set; }
        public decimal CostoUnitario { get; set; }
    }

}
