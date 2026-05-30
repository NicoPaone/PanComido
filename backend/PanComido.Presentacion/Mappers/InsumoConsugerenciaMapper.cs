using PanComido.Presentacion.DTOs;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class InsumoConsugerenciaMapper
    {
        public InsumoParaReponerResponseDto aDto(DOM.InusmoConSugerencia insumoSugerido)
        {
            return new InsumoParaReponerResponseDto
            {
                Id = insumoSugerido.Id,
                Nombre = insumoSugerido.Nombre,
                UnidadMedida = insumoSugerido.UnidadMedida,
                StockActual = insumoSugerido.StockActual,
                CantidadSugerida = insumoSugerido.CantidadSugerida,
                EstadoStock = insumoSugerido.EstadoStock?.ToString()
            };
        }

        public List<InsumoParaReponerResponseDto> aListaDto(
            List<DOM.InusmoConSugerencia> insumosSugeridos)
        {
            return insumosSugeridos
                .Select(i => aDto(i))
                .ToList();
        }
    }
}
