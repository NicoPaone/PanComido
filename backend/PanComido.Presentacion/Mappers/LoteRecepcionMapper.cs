using PanComido.Presentacion.DTOs.Pedidos;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class LoteRecepcionMapper
    {
        public PreRecepcionItemResponseDto aDto(DOM.RecepcionItemSugerido recepcionItemSugerido)
        {
            return new PreRecepcionItemResponseDto
            {
                InsumoId = recepcionItemSugerido.InsumoId,
                NombreInsumo = recepcionItemSugerido.NombreInsumo,
                Cantidad = recepcionItemSugerido.Cantidad,
                NombreLote = recepcionItemSugerido.NombreLote,
                BodegaIdSug = recepcionItemSugerido.BodegaIdSug,
                FechaVencimientoSug = recepcionItemSugerido.FechaVencimientoSug.ToString("dd/MM/yyyy")
            };
        }
        public List<PreRecepcionItemResponseDto> aListaDto(List<DOM.RecepcionItemSugerido> items)
        {
            return items.Select(aDto).ToList();
        }

        public DOM.Lote aDominio(RecibirPedidoItemDto itemPedidoARecibirDto)
        {
            return new DOM.Lote
                {
                    Nombre = itemPedidoARecibirDto.NombreLote,
                    InsumoId = itemPedidoARecibirDto.InsumoId,
                    Cantidad = itemPedidoARecibirDto.Cantidad,
                    BodegaId = itemPedidoARecibirDto.BodegaId,
                    FechaAdquisicion = DateOnly.FromDateTime(DateTime.UtcNow),
                    FechaVencimiento = DateOnly.Parse(itemPedidoARecibirDto.FechaVencimiento)
                };
        }

        public List<DOM.Lote> aListaDominio(List<RecibirPedidoItemDto> itemsPedidoARecibirDto)
        {
            return itemsPedidoARecibirDto.Select(aDominio).ToList();
        }
    }
}
