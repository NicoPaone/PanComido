using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class ArticuloComandum
{
    public int Id { get; set; }

    public int ComandaId { get; set; }

    public int ArticuloId { get; set; }

    public int Cantidad { get; set; }

    public bool Entregado { get; set; }

    public string? ObservacionesIngrediente { get; set; }

    public string? ObservacionesGenerales { get; set; }

    public virtual Articulo Articulo { get; set; } = null!;

    public virtual Comandum Comanda { get; set; } = null!;
}
