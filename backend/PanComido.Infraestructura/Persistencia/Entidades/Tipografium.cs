using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("tipografia")]
[Index("Nombre", Name = "tipografia_nombre_key", IsUnique = true)]
public partial class Tipografium
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("nombre")]
    public string Nombre { get; set; } = null!;

    [InverseProperty("Tipografia")]
    public virtual ICollection<Restaurante> Restaurantes { get; set; } = new List<Restaurante>();
}
