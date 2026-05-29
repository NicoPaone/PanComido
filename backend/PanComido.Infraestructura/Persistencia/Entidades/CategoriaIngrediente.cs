using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("categoria_ingrediente")]
[Index("Descripcion", Name = "categoria_ingrediente_descripcion_key", IsUnique = true)]
public partial class CategoriaIngrediente
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; } = null!;

    [InverseProperty("CategoriaIngrediente")]
    public virtual ICollection<Ingrediente> Ingredientes { get; set; } = new List<Ingrediente>();

    [ForeignKey("CategoriaIngredienteId")]
    [InverseProperty("CategoriaIngredientes")]
    public virtual ICollection<CategoriaProveedor> CategoriaProveedors { get; set; } = new List<CategoriaProveedor>();
}
