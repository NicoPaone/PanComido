using PanComido.Presentacion.DTOs.Lotes;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class LoteMapper
    {
        public LoteResponseDto aDto(DOM.Lote lote)
        {
            return new LoteResponseDto
            {
                Id = lote.Id,
                FechaVencimiento = lote.FechaVencimiento,
                Cantidad = lote.Cantidad
            };
        }

        public List<LoteResponseDto> aListaDto(
            List<DOM.Lote> lotes)
        {
            return lotes
                .Select(l => aDto(l))
                .ToList();
        }

        public Dictionary<int, List<LoteResponseDto>> aDiccionarioDto(
            Dictionary<int, List<DOM.Lote>> lotesPorInsumo)
        {
            return lotesPorInsumo
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => aListaDto(kvp.Value)
                );
        }

    }
}
