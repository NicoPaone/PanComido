using PanComido.Presentacion.DTOs.Pedidos;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class InsumoConsugerenciaMapper
    {
        public InsumoParaReponerResponseDto aDto(DOM.InsumoConSugerencia insumoSugerido)
        {
            return new InsumoParaReponerResponseDto
            {
                Id = insumoSugerido.Id,
                Nombre = insumoSugerido.Nombre,
                UnidadMedida = insumoSugerido.UnidadMedida,
                StockActual = insumoSugerido.StockActual,
                CantidadSugerida = insumoSugerido.CantidadSugerida,
                EstadoStock = insumoSugerido.EstadoStock?.ToString(),
                PrecioUnitario = insumoSugerido.PrecioUnitario,
                PrecioTotalSugerido = insumoSugerido.PrecioTotalSugerido
            };
        }

        public List<InsumoParaReponerResponseDto> aListaDto(
            List<DOM.InsumoConSugerencia> insumosSugeridos)
        {
            return insumosSugeridos
                .Select(i => aDto(i))
                .ToList();
        }
    }
}
