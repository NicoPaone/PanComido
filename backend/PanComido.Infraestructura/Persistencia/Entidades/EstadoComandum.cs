using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("estado_comanda")]
[Index("Descripcion", Name = "estado_comanda_descripcion_key", IsUnique = true)]
public partial class EstadoComandum
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; } = null!;

    [InverseProperty("EstadoComanda")]
    public virtual ICollection<Comandum> Comanda { get; set; } = new List<Comandum>();
}
