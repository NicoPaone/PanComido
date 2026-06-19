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

    [Column("entregado")]
    public bool Entregado { get; set; }

    [Column("observaciones_generales")]
    public string? ObservacionesGenerales { get; set; }

    [Column("nombre_comensal")]
    public string NombreComensal { get; set; } = null!;

    [ForeignKey("ArticuloId")]
    [InverseProperty("ArticuloComanda")]
    public virtual Articulo Articulo { get; set; } = null!;

    [InverseProperty("ArticuloComanda")]
    public virtual ICollection<ArticuloComandaIngredienteExcluido> ArticuloComandaIngredienteExcluidos { get; set; } = new List<ArticuloComandaIngredienteExcluido>();

    [ForeignKey("ComandaId")]
    [InverseProperty("ArticuloComanda")]
    public virtual Comandum Comanda { get; set; } = null!;
}
