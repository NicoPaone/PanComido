using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("estado_mesa")]
[Index("Descripcion", Name = "estado_mesa_descripcion_key", IsUnique = true)]
public partial class EstadoMesa
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; } = null!;

    [InverseProperty("EstadoMesa")]
    public virtual ICollection<Mesa> Mesas { get; set; } = new List<Mesa>();
}
