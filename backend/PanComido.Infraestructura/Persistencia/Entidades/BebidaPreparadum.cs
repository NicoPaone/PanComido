using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("bebida_preparada")]
public partial class BebidaPreparadum
{
    [Key]
    [Column("id_articulo")]
    public int IdArticulo { get; set; }

    [InverseProperty("BebidaPreparada")]
    public virtual ICollection<BebidaPreparadaInsumo> BebidaPreparadaInsumos { get; set; } = new List<BebidaPreparadaInsumo>();

    [ForeignKey("IdArticulo")]
    [InverseProperty("BebidaPreparadum")]
    public virtual Articulo IdArticuloNavigation { get; set; } = null!;
}
