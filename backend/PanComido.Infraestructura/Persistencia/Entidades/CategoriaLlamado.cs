using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("categoria_llamado")]
[Index("Descripcion", Name = "categoria_llamado_descripcion_key", IsUnique = true)]
public partial class CategoriaLlamado
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; } = null!;

    [InverseProperty("CategoriaLlamado")]
    public virtual ICollection<Llamado> Llamados { get; set; } = new List<Llamado>();
}
