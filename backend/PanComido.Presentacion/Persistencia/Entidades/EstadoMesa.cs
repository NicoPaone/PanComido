using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class EstadoMesa
{
    public int Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Mesa> Mesas { get; set; } = new List<Mesa>();
}
