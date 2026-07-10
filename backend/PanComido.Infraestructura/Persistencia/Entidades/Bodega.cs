using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("bodega")]
public partial class Bodega
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("restaurante_id")]
    public int RestauranteId { get; set; }

    [Column("tipo_bodega_id")]
    public int TipoBodegaId { get; set; }

    [Column("nombre")]
    public string Nombre { get; set; } = null!;

    [Column("eliminado")]
    public bool Eliminado { get; set; }

    [InverseProperty("Bodega")]
    public virtual ICollection<Lote> Lotes { get; set; } = new List<Lote>();

    [ForeignKey("RestauranteId")]
    [InverseProperty("Bodegas")]
    public virtual Restaurante Restaurante { get; set; } = null!;

    [ForeignKey("TipoBodegaId")]
    [InverseProperty("Bodegas")]
    public virtual TipoBodega TipoBodega { get; set; } = null!;
}
