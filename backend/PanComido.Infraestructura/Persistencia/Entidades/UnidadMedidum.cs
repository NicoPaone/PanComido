using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("unidad_medida")]
[Index("Nombre", Name = "unidad_medida_nombre_key", IsUnique = true)]
public partial class UnidadMedidum
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("nombre")]
    public string Nombre { get; set; } = null!;

    [InverseProperty("UnidadMedida")]
    public virtual ICollection<Insumo> Insumos { get; set; } = new List<Insumo>();
}
