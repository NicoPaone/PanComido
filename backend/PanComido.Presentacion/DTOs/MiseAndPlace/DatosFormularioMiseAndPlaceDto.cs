using PanComido.Presentacion.DTOs.Insumos;
using PanComido.Presentacion.DTOs.UnidadesDeMedida;
using PanComido.Presentacion.DTOs.Bodegas;
using System.Collections.Generic;

namespace PanComido.Presentacion.DTOs.MiseAndPlace
{
    public class DatosFormularioMiseAndPlaceDto
    {
        public List<IngredienteMiseAndPlaceResponseDto> Ingredientes { get; set; } = new List<IngredienteMiseAndPlaceResponseDto>();
        public List<CategoriaLightDto> Categorias { get; set; } = new List<CategoriaLightDto>();
        public List<UnidadMedidaResponseDto> UnidadesMedida { get; set; } = new List<UnidadMedidaResponseDto>();
        public List<BodegaLightDto> Bodegas { get; set; } = new List<BodegaLightDto>();
    }

    public class CategoriaLightDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }

    public class BodegaLightDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
