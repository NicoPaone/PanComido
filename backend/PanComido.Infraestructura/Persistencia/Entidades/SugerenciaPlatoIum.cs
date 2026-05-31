using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("sugerencia_plato_ia")]
public partial class SugerenciaPlatoIum
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("restaurante_id")]
    public int RestauranteId { get; set; }

    [Column("json", TypeName = "jsonb")]
    public string Json { get; set; } = null!;

    [ForeignKey("RestauranteId")]
    [InverseProperty("SugerenciaPlatoIa")]
    public virtual Restaurante Restaurante { get; set; } = null!;
}
