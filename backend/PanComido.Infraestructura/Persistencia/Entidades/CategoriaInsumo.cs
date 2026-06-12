using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("categoria_insumo")]
[Index("Descripcion", Name = "categoria_insumo_descripcion_key", IsUnique = true)]
public partial class CategoriaInsumo
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; } = null!;

    [Column("tipo_aplica")]
    public int TipoAplica { get; set; }

    [InverseProperty("CategoriaInsumo")]
    public virtual ICollection<Insumo> Insumos { get; set; } = new List<Insumo>();

    [InverseProperty("CategoriaInsumo")]
    public virtual ICollection<PorcentajeCategoriaBebidum> PorcentajeCategoriaBebida { get; set; } = new List<PorcentajeCategoriaBebidum>();

    [ForeignKey("CategoriaInsumoId")]
    [InverseProperty("CategoriaInsumos")]
    public virtual ICollection<Proveedor> Proveedors { get; set; } = new List<Proveedor>();
}
