using PanComido.Presentacion.DTOs.FamiliaTipografica;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class FamiliaTipograficaMapper
    {
        public FamiliaTipograficaResponseDto aDto(DOM.FamiliaTipografica familiaTipografica)
        {
            return new FamiliaTipograficaResponseDto
            {
                Id = familiaTipografica.Id,
                Categoria = familiaTipografica.Categoria,
                TipografiaTitulo = familiaTipografica.TipografiaTitulo,
                TipografiaCuerpo = familiaTipografica.TipografiaCuerpo
            };
        }

        public List<FamiliaTipograficaResponseDto> aListaDto(List<DOM.FamiliaTipografica> familiasTipograficas)
        {
            return familiasTipograficas.Select(f => aDto(f)).ToList();
                }
    }
}
