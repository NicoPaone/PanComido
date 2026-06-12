using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Articulo
{
    public int Id { get; set; }

    public int? CartaId { get; set; }

    public int RestauranteId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal? PrecioVentaFinal { get; set; }

    public decimal? PrecioGanancia { get; set; }

    public decimal? PrecioPromocional { get; set; }

    public string? UrlImagen { get; set; }

    public bool Eliminado { get; set; }

    public virtual ICollection<ArticuloComandum> ArticuloComanda { get; set; } = new List<ArticuloComandum>();

    public virtual Cartum? Carta { get; set; }

    public virtual Insumo? Insumo { get; set; }

    public virtual Plato? Plato { get; set; }

    public virtual Restaurante Restaurante { get; set; } = null!;

    public virtual ICollection<ConfiguracionArticulo> ConfiguracionArticulos { get; set; } = new List<ConfiguracionArticulo>();
}
