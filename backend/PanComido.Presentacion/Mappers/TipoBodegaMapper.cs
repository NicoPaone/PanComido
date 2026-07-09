using PanComido.Presentacion.DTOs.Bodegas;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class TipoBodegaMapper
    {
        public TipoBodegaResponseDto aDto(DOM.TipoBodega tipoBodega)
        {
            if (tipoBodega == null) return null;
            return new TipoBodegaResponseDto
            {
                Id = tipoBodega.Id,
                Descripcion = tipoBodega.Descripcion
            };
        }

        public List<TipoBodegaResponseDto> aListaDto(List<DOM.TipoBodega> tiposBodega)
        {
            if (tiposBodega == null) return new List<TipoBodegaResponseDto>();
            return tiposBodega.Select(t => aDto(t)).ToList();
        }
    }
}
