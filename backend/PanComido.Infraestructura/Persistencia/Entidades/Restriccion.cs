using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("restriccion")]
[Index("Descripcion", Name = "restriccion_descripcion_key", IsUnique = true)]
public partial class Restriccion
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; } = null!;

    [ForeignKey("RestriccionId")]
    [InverseProperty("Restriccions")]
    public virtual ICollection<Plato> Platos { get; set; } = new List<Plato>();
}
