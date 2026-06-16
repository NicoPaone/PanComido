using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("estado_pago")]
[Index("Descripcion", Name = "estado_pago_descripcion_key", IsUnique = true)]
public partial class EstadoPago
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; } = null!;

    [InverseProperty("EstadoPago")]
    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
