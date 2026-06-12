using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[PrimaryKey("RestauranteId", "CategoriaInsumoId")]
[Table("porcentaje_categoria_bebida")]
public partial class PorcentajeCategoriaBebidum
{
    [Key]
    [Column("restaurante_id")]
    public int RestauranteId { get; set; }

    [Key]
    [Column("categoria_insumo_id")]
    public int CategoriaInsumoId { get; set; }

    [Column("porcentaje")]
    public decimal Porcentaje { get; set; }

    [ForeignKey("CategoriaInsumoId")]
    [InverseProperty("PorcentajeCategoriaBebida")]
    public virtual CategoriaInsumo CategoriaInsumo { get; set; } = null!;

    [ForeignKey("RestauranteId")]
    [InverseProperty("PorcentajeCategoriaBebida")]
    public virtual Restaurante Restaurante { get; set; } = null!;
}
