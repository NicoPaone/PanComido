using PanComido.Presentacion.DTOs.ReglaTiempoExtra;
using DOM = PanComido.Dominio.Entidades;
using System.Collections.Generic;
using System.Linq;

namespace PanComido.Presentacion.Mappers
{
    public class ReglaTiempoExtraMapper
    {
        public ReglaTiempoExtraResponseDto aDto(DOM.ReglaTiempoExtra dominio)
        {
            if (dominio == null) return null;
            return new ReglaTiempoExtraResponseDto
            {
                Id = dominio.Id,
                PorcentajeOcupacionHasta = dominio.PorcentajeOcupacionHasta,
                MinutosExtra = dominio.MinutosExtra
            };
        }

        public List<ReglaTiempoExtraResponseDto> aListaDto(IEnumerable<DOM.ReglaTiempoExtra> dominios)
        {
            return dominios?.Select(aDto).ToList() ?? new List<ReglaTiempoExtraResponseDto>();
        }

        public DOM.ReglaTiempoExtra aDominio(GuardarReglaTiempoExtraRequestDto dto, int restauranteId)
        {
            return new DOM.ReglaTiempoExtra
            {
                RestauranteId = restauranteId,
                PorcentajeOcupacionHasta = dto.PorcentajeOcupacionHasta,
                MinutosExtra = dto.MinutosExtra
            };
        }
    }
}
