using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("ingrediente")]
public partial class Ingrediente
{
    [Key]
    [Column("id_insumo")]
    public int IdInsumo { get; set; }

    [ForeignKey("IdInsumo")]
    [InverseProperty("Ingrediente")]
    public virtual Insumo IdInsumoNavigation { get; set; } = null!;

    [InverseProperty("IdIngredienteNavigation")]
    public virtual IngredientePreparado? IngredientePreparado { get; set; }

    [InverseProperty("Ingrediente")]
    public virtual ICollection<PlatoIngrediente> PlatoIngredientes { get; set; } = new List<PlatoIngrediente>();

    [ForeignKey("IngredienteId")]
    [InverseProperty("Ingredientes")]
    public virtual ICollection<IngredientePreparado> IngredientePreparados { get; set; } = new List<IngredientePreparado>();
}
