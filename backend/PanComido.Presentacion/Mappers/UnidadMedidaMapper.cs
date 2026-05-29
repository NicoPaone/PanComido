using PanComido.Presentacion.DTOs;
using DOM = PanComido.Dominio.Entidades;


namespace PanComido.Presentacion.Mappers
{
    public class UnidadMedidaMapper
    {
        public UnidadMedidaResponseDto aDto(DOM.UnidadMedida unidadMedida)
        {
            return new UnidadMedidaResponseDto
            {
                Id = unidadMedida.Id,
                Nombre = unidadMedida.Nombre,
            };
        }
        public List<UnidadMedidaResponseDto> aListaDto(
            List<DOM.UnidadMedida> unidadesDeMedida)
        {
            return unidadesDeMedida
                .Select(u => aDto(u))
                .ToList();
        }
    }
}
