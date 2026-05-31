using PanComido.Dominio.Entidades;
using PanComido.Infraestructura.Persistencia.Entidades;
using PanComido.Presentacion.DTOs.Pedidos;
using DOM = PanComido.Dominio.Entidades;


namespace PanComido.Presentacion.Mappers
{
    public class PedidoMapper
    {
        public PedidoResponseDto aDto(DOM.Pedido pedido)
        {
            return new PedidoResponseDto
            {
                Id = pedido.Id,
                Fecha = pedido.Fecha.ToString("dd/MM/yyyy"),
                Estado = pedido.Estado,
                ItemsInsumo = pedido.ItemsInsumo.Select(item => new PedidoInsumoResponseDto
                {
                    InsumoId = item.InsumoId,
                    NombreInsumo = item.NombreInsumo,
                    Cantidad = item.Cantidad,
                    PrecioCompra = item.PrecioCompra
                }).ToList()
            };
        }

        public List<PedidoResponseDto> aListaDto(List<DOM.Pedido> pedidos)
        {
            return pedidos.Select(aDto).ToList();
        }

        public CrearPedidoResponseDto aDtoCrear(DOM.Pedido pedido)
        {
            return new CrearPedidoResponseDto
            {
                Id = pedido.Id,
                Fecha = pedido.Fecha.ToString("dd/MM/yyyy"),
                Estado = pedido.Estado,
                ProveedorId = pedido.ProveedorId,
                ProveedorNombre = pedido.ProveedorNombre,
                ProveedorTelefono = pedido.ProveedorTelefono,
                ItemsInsumo = pedido.ItemsInsumo.Select(item => new PedidoInsumoResponseDto
                {
                    InsumoId = item.InsumoId,
                    NombreInsumo = item.NombreInsumo,
                    Cantidad = item.Cantidad,
                    PrecioCompra = item.PrecioCompra
                }).ToList()
            };
        }

    }
}
