using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[PrimaryKey("RestauranteId", "CategoriaPlatoId")]
[Table("porcentaje_categoria_plato")]
public partial class PorcentajeCategoriaPlato
{
    [Key]
    [Column("restaurante_id")]
    public int RestauranteId { get; set; }

    [Key]
    [Column("categoria_plato_id")]
    public int CategoriaPlatoId { get; set; }

    [Column("porcentaje")]
    public decimal Porcentaje { get; set; }

    [ForeignKey("CategoriaPlatoId")]
    [InverseProperty("PorcentajeCategoriaPlatos")]
    public virtual CategoriaPlato CategoriaPlato { get; set; } = null!;

    [ForeignKey("RestauranteId")]
    [InverseProperty("PorcentajeCategoriaPlatos")]
    public virtual Restaurante Restaurante { get; set; } = null!;
}
