using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("categoria_proveedor")]
[Index("Descripcion", Name = "categoria_proveedor_descripcion_key", IsUnique = true)]
public partial class CategoriaProveedor
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; } = null!;

    [ForeignKey("CategoriaProveedorId")]
    [InverseProperty("CategoriaProveedors")]
    public virtual ICollection<CategoriaIngrediente> CategoriaIngredientes { get; set; } = new List<CategoriaIngrediente>();

    [ForeignKey("CategoriaProveedorId")]
    [InverseProperty("CategoriaProveedors")]
    public virtual ICollection<Proveedor> Proveedors { get; set; } = new List<Proveedor>();
}
