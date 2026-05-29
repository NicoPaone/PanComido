using PanComido.Presentacion.DTOs;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class InsumoMapper
    {
        public InsumoResponseDto aDto(DOM.Insumo insumo)
        {
            return new InsumoResponseDto
            {
                Id = insumo.Id,
                Nombre = insumo.Nombre,
                StockActual = insumo.StockActual,
                UnidadMedida = insumo.UnidadMedida,
                Vencimiento = insumo.Vencimiento?.ToString("dd/MM/yyyy"),
                StockMinimo = insumo.StockMinimo,
                EstadoStock = insumo.EstadoStock?.ToString(),
                Tipo = insumo.Tipo.ToString(),
                Categoria = insumo.Categoria,
            };
        }
        public List<InsumoResponseDto> aListaDto(
            List<DOM.Insumo> insumos)
        {
            return insumos
                .Select(i => aDto(i))
                .ToList();
        }
    }
}
