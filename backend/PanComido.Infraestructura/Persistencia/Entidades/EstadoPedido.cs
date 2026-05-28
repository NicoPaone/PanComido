using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("estado_pedido")]
[Index("Descripcion", Name = "estado_pedido_descripcion_key", IsUnique = true)]
public partial class EstadoPedido
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; } = null!;

    [InverseProperty("EstadoPedido")]
    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
