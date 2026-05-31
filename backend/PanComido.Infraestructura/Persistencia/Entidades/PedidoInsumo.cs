using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[PrimaryKey("PedidoId", "InsumoId")]
[Table("pedido_insumo")]
public partial class PedidoInsumo
{
    [Key]
    [Column("pedido_id")]
    public int PedidoId { get; set; }

    [Key]
    [Column("insumo_id")]
    public int InsumoId { get; set; }

    [Column("precio_compra")]
    public decimal PrecioCompra { get; set; }

    [Column("cantidad")]
    public decimal Cantidad { get; set; }

    [ForeignKey("InsumoId")]
    [InverseProperty("PedidoInsumos")]
    public virtual Insumo Insumo { get; set; } = null!;

    [ForeignKey("PedidoId")]
    [InverseProperty("PedidoInsumos")]
    public virtual Pedido Pedido { get; set; } = null!;
}
