using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class DimensionMesa
{
    public int Id { get; set; }

    public string? Imagen { get; set; }

    public string Forma { get; set; } = null!;

    public virtual ICollection<Mesa> Mesas { get; set; } = new List<Mesa>();
}
