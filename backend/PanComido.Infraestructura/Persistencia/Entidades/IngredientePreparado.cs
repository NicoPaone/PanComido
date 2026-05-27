using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("ingrediente_preparado")]
public partial class IngredientePreparado
{
    [Key]
    [Column("id_ingrediente")]
    public int IdIngrediente { get; set; }

    [ForeignKey("IdIngrediente")]
    [InverseProperty("IngredientePreparado")]
    public virtual Ingrediente IdIngredienteNavigation { get; set; } = null!;

    [ForeignKey("IngredientePreparadoId")]
    [InverseProperty("IngredientePreparados")]
    public virtual ICollection<Ingrediente> Ingredientes { get; set; } = new List<Ingrediente>();
}
