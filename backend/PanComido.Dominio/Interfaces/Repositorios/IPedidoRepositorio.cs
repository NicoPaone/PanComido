using PanComido.Dominio.Entidades;


namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IPedidoRepositorio
    {
        Task<DateOnly?> ObtenerFechaUltimoPedidoDeProveedorAsync(int proveedorId);
        Task<List<Pedido>> ObtenerPedidosPorProveedorAsync(int proveedorId);
        Task<Pedido> CrearPedidoAsync(Pedido pedido);
        Task<decimal> ObtenerUltimoPrecioCompraUnitarioAsync(int insumoId, int proveedorId);
        Task<Pedido> EnviarPedidoAsync(int pedidoId, List<PedidoInsumo> itemsNuevos);
        Task<Pedido> ObtenerPedidoPorIdAsync(int pedidoId);
        Task MarcarComoRecibidoAsync(int pedidoId);
    }
}
