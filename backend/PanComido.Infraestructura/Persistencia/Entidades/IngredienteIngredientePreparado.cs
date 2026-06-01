using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[PrimaryKey("IngredienteId", "IngredientePreparadoId")]
[Table("ingrediente_ingrediente_preparado")]
public partial class IngredienteIngredientePreparado
{
    [Key]
    [Column("ingrediente_id")]
    public int IngredienteId { get; set; }

    [Key]
    [Column("ingrediente_preparado_id")]
    public int IngredientePreparadoId { get; set; }

    [Column("cantidad")]
    public decimal Cantidad { get; set; }

    [ForeignKey("IngredienteId")]
    [InverseProperty("IngredienteIngredientePreparados")]
    public virtual Ingrediente Ingrediente { get; set; } = null!;

    [ForeignKey("IngredientePreparadoId")]
    [InverseProperty("IngredienteIngredientePreparados")]
    public virtual IngredientePreparado IngredientePreparado { get; set; } = null!;
}
