using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Pedido
{
    public int Id { get; set; }

    public int ProveedorId { get; set; }

    public int EstadoPedidoId { get; set; }

    public DateOnly Fecha { get; set; }

    public virtual EstadoPedido EstadoPedido { get; set; } = null!;

    public virtual ICollection<PedidoInsumo> PedidoInsumos { get; set; } = new List<PedidoInsumo>();

    public virtual Proveedor Proveedor { get; set; } = null!;
}
