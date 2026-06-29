using PanComido.Dominio.Entidades.Enums;

namespace PanComido.Dominio.Entidades
{
    public class Pedido
    {
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public string ProveedorNombre { get; set; }
        public string ProveedorTelefono { get; set; }
        public DateOnly Fecha { get; set; }
        public EstadoPedido Estado { get; set; }
        public List<PedidoInsumo> ItemsInsumo { get; set; } = new();
    }
}
