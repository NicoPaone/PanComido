using PanComido.Dominio.Entidades;


namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IPedidoRepositorio
    {
        Task<DateOnly?> ObtenerFechaUltimoPedidoDeProveedorAsync(int proveedorId);
        Task<List<Pedido>> ObtenerPedidosPorProveedorAsync(int proveedorId);
    }
}
