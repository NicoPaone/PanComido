using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class PedidoInsumo
{
    public int PedidoId { get; set; }

    public int InsumoId { get; set; }

    public decimal PrecioCompra { get; set; }

    public decimal Cantidad { get; set; }

    public virtual Insumo Insumo { get; set; } = null!;

    public virtual Pedido Pedido { get; set; } = null!;
}
