using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("articulo")]
public partial class Articulo
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("carta_id")]
    public int? CartaId { get; set; }

    [Column("restaurante_id")]
    public int RestauranteId { get; set; }

    [Column("nombre")]
    public string Nombre { get; set; } = null!;

    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [Column("precio_venta_final")]
    public decimal? PrecioVentaFinal { get; set; }

    [Column("precio_ganancia")]
    public decimal? PrecioGanancia { get; set; }

    [Column("precio_promocional")]
    public decimal? PrecioPromocional { get; set; }

    [Column("url_imagen")]
    public string? UrlImagen { get; set; }

    [Column("eliminado")]
    public bool Eliminado { get; set; }

    [Column("es_precio_manual")]
    public bool EsPrecioManual { get; set; }

    [InverseProperty("Articulo")]
    public virtual ICollection<ArticuloComandum> ArticuloComanda { get; set; } = new List<ArticuloComandum>();

    [InverseProperty("IdArticuloNavigation")]
    public virtual BebidaPreparadum? BebidaPreparadum { get; set; }

    [ForeignKey("CartaId")]
    [InverseProperty("Articulos")]
    public virtual Cartum? Carta { get; set; }

    [InverseProperty("IdArticuloNavigation")]
    public virtual Insumo? Insumo { get; set; }

    [InverseProperty("IdArticuloNavigation")]
    public virtual Plato? Plato { get; set; }

    [ForeignKey("RestauranteId")]
    [InverseProperty("Articulos")]
    public virtual Restaurante Restaurante { get; set; } = null!;

    [ForeignKey("ArticuloId")]
    [InverseProperty("Articulos")]
    public virtual ICollection<ConfiguracionArticulo> ConfiguracionArticulos { get; set; } = new List<ConfiguracionArticulo>();
}
