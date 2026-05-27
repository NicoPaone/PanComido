using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("tipo_bodega")]
[Index("Descripcion", Name = "tipo_bodega_descripcion_key", IsUnique = true)]
public partial class TipoBodega
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; } = null!;

    [InverseProperty("TipoBodega")]
    public virtual ICollection<Bodega> Bodegas { get; set; } = new List<Bodega>();
}
