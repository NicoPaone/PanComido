using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[PrimaryKey("PlatoId", "IngredienteId")]
[Table("plato_ingrediente")]
public partial class PlatoIngrediente
{
    [Key]
    [Column("plato_id")]
    public int PlatoId { get; set; }

    [Key]
    [Column("ingrediente_id")]
    public int IngredienteId { get; set; }

    [Column("opcional")]
    public bool Opcional { get; set; }

    [ForeignKey("IngredienteId")]
    [InverseProperty("PlatoIngredientes")]
    public virtual Ingrediente Ingrediente { get; set; } = null!;

    [ForeignKey("PlatoId")]
    [InverseProperty("PlatoIngredientes")]
    public virtual Plato Plato { get; set; } = null!;
}
