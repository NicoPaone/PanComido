using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("pedido")]
public partial class Pedido
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("proveedor_id")]
    public int ProveedorId { get; set; }

    [Column("estado_pedido_id")]
    public int EstadoPedidoId { get; set; }

    [Column("fecha")]
    public DateOnly Fecha { get; set; }

    [ForeignKey("EstadoPedidoId")]
    [InverseProperty("Pedidos")]
    public virtual EstadoPedido EstadoPedido { get; set; } = null!;

    [InverseProperty("Pedido")]
    public virtual ICollection<PedidoInsumo> PedidoInsumos { get; set; } = new List<PedidoInsumo>();

    [ForeignKey("ProveedorId")]
    [InverseProperty("Pedidos")]
    public virtual Proveedor Proveedor { get; set; } = null!;
}
