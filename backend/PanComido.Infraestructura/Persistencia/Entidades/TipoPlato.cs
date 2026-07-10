using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("tipo_plato")]
[Index("Descripcion", Name = "tipo_plato_descripcion_key", IsUnique = true)]
public partial class TipoPlato
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; } = null!;

    [InverseProperty("TipoPlato")]
    public virtual ICollection<Plato> Platos { get; set; } = new List<Plato>();
}
