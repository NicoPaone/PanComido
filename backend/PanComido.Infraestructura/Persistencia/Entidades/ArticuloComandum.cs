using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("articulo_comanda")]
public partial class ArticuloComandum
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("comanda_id")]
    public int ComandaId { get; set; }

    [Column("articulo_id")]
    public int ArticuloId { get; set; }

    [Column("cantidad")]
    public int Cantidad { get; set; }

    [ForeignKey("ArticuloId")]
    [InverseProperty("ArticuloComanda")]
    public virtual Articulo Articulo { get; set; } = null!;

    [ForeignKey("ComandaId")]
    [InverseProperty("ArticuloComanda")]
    public virtual Comandum Comanda { get; set; } = null!;
}
