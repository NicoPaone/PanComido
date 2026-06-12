using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("categoria_plato")]
[Index("Descripcion", Name = "categoria_plato_descripcion_key", IsUnique = true)]
public partial class CategoriaPlato
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; } = null!;

    [InverseProperty("CategoriaPlato")]
    public virtual ICollection<Plato> Platos { get; set; } = new List<Plato>();

    [InverseProperty("CategoriaPlato")]
    public virtual ICollection<PorcentajeCategoriaPlato> PorcentajeCategoriaPlatos { get; set; } = new List<PorcentajeCategoriaPlato>();
}
