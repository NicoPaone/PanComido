using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[PrimaryKey("LoteId", "BodegaId")]
[Table("lote_bodega")]
public partial class LoteBodega
{
    [Key]
    [Column("lote_id")]
    public int LoteId { get; set; }

    [Key]
    [Column("bodega_id")]
    public int BodegaId { get; set; }

    [Column("cantidad_almacenada")]
    public decimal CantidadAlmacenada { get; set; }

    [ForeignKey("BodegaId")]
    [InverseProperty("LoteBodegas")]
    public virtual Bodega Bodega { get; set; } = null!;

    [ForeignKey("LoteId")]
    [InverseProperty("LoteBodegas")]
    public virtual Lote Lote { get; set; } = null!;
}
