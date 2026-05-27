using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("categoria_bebida")]
[Index("Descripcion", Name = "categoria_bebida_descripcion_key", IsUnique = true)]
public partial class CategoriaBebidum
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; } = null!;

    [InverseProperty("CategoriaBebida")]
    public virtual ICollection<Bebidum> Bebida { get; set; } = new List<Bebidum>();
}
