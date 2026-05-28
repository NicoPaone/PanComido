using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("dimension_mesa")]
public partial class DimensionMesa
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("imagen")]
    public string? Imagen { get; set; }

    [Column("forma")]
    public string Forma { get; set; } = null!;

    [InverseProperty("DimensionMesa")]
    public virtual ICollection<Mesa> Mesas { get; set; } = new List<Mesa>();
}
