using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("plato")]
public partial class Plato
{
    [Key]
    [Column("id_articulo")]
    public int IdArticulo { get; set; }

    [Column("tipo_plato_id")]
    public int TipoPlatoId { get; set; }

    [Column("categoria_plato_id")]
    public int CategoriaPlatoId { get; set; }

    [Column("tiempo_preparacion_base")]
    public int TiempoPreparacionBase { get; set; }

    [Column("destacado")]
    public bool Destacado { get; set; }

    [Column("sugerencia")]
    public bool? Sugerencia { get; set; }

    [ForeignKey("CategoriaPlatoId")]
    [InverseProperty("Platos")]
    public virtual CategoriaPlato CategoriaPlato { get; set; } = null!;

    [ForeignKey("IdArticulo")]
    [InverseProperty("Plato")]
    public virtual Articulo IdArticuloNavigation { get; set; } = null!;

    [InverseProperty("Plato")]
    public virtual ICollection<PlatoIngrediente> PlatoIngredientes { get; set; } = new List<PlatoIngrediente>();

    [ForeignKey("TipoPlatoId")]
    [InverseProperty("Platos")]
    public virtual TipoPlato TipoPlato { get; set; } = null!;

    [ForeignKey("PlatoId")]
    [InverseProperty("Platos")]
    public virtual ICollection<Restriccion> Restriccions { get; set; } = new List<Restriccion>();
}
