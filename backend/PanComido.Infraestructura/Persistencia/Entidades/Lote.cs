using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("lote")]
public partial class Lote
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("insumo_id")]
    public int InsumoId { get; set; }

    [Column("bodega_id")]
    public int BodegaId { get; set; }

    [Column("nombre")]
    public string Nombre { get; set; } = null!;

    [Column("cantidad")]
    public decimal Cantidad { get; set; }

    [Column("fecha_adquisicion")]
    public DateOnly FechaAdquisicion { get; set; }

    [Column("fecha_vencimiento")]
    public DateOnly? FechaVencimiento { get; set; }

    [Column("eliminado")]
    public bool Eliminado { get; set; }

    [ForeignKey("BodegaId")]
    [InverseProperty("Lotes")]
    public virtual Bodega Bodega { get; set; } = null!;

    [ForeignKey("InsumoId")]
    [InverseProperty("Lotes")]
    public virtual Insumo Insumo { get; set; } = null!;
}
