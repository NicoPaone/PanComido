using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[PrimaryKey("BebidaPreparadaId", "InsumoId")]
[Table("bebida_preparada_insumo")]
public partial class BebidaPreparadaInsumo
{
    [Key]
    [Column("bebida_preparada_id")]
    public int BebidaPreparadaId { get; set; }

    [Key]
    [Column("insumo_id")]
    public int InsumoId { get; set; }

    [Column("cantidad")]
    public decimal Cantidad { get; set; }

    [ForeignKey("BebidaPreparadaId")]
    [InverseProperty("BebidaPreparadaInsumos")]
    public virtual BebidaPreparadum BebidaPreparada { get; set; } = null!;

    [ForeignKey("InsumoId")]
    [InverseProperty("BebidaPreparadaInsumos")]
    public virtual Insumo Insumo { get; set; } = null!;
}
