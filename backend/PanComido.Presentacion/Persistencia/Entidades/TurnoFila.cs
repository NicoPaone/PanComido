using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class TurnoFila
{
    public int Id { get; set; }

    public int FilaVirtualId { get; set; }

    public int Numero { get; set; }

    public virtual FilaVirtual FilaVirtual { get; set; } = null!;
}
