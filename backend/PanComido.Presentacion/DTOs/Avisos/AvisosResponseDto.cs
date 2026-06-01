using PanComido.Presentacion.DTOs.Insumos;
using PanComido.Presentacion.DTOs.Lotes;

namespace PanComido.Presentacion.DTOs.Avisos
{
    public class AvisosResponseDto
    {
        public List<InsumoResponseDto> InsumosConStockCritico { get; set; } = new List<InsumoResponseDto>();
        public Dictionary<int, List<LoteResponseDto>> InsumosConVencimientoProximo { get; set; } = new Dictionary<int, List<LoteResponseDto>>();
        
    }
}