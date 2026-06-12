using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Plato
{
    public int IdArticulo { get; set; }

    public int TipoPlatoId { get; set; }

    public int CategoriaPlatoId { get; set; }

    public int TiempoPreparacionBase { get; set; }

    public bool Destacado { get; set; }

    public bool Sugerencia { get; set; }

    public virtual CategoriaPlato CategoriaPlato { get; set; } = null!;

    public virtual Articulo IdArticuloNavigation { get; set; } = null!;

    public virtual ICollection<PlatoIngrediente> PlatoIngredientes { get; set; } = new List<PlatoIngrediente>();

    public virtual TipoPlato TipoPlato { get; set; } = null!;

    public virtual ICollection<Restriccion> Restriccions { get; set; } = new List<Restriccion>();
}
