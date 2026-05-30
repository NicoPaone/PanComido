using PanComido.Presentacion.DTOs.Insumos;

namespace PanComido.Presentacion.DTOs.Bodegas
{
    public class BodegaConInsumosResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public string TipoBodega { get; set; }
        public List<InsumoResponseDto> Insumos { get; set; } = new List<InsumoResponseDto>();
    }
}
