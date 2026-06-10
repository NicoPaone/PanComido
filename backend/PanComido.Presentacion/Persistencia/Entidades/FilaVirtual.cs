using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class FilaVirtual
{
    public int Id { get; set; }

    public int RestauranteId { get; set; }

    public bool Habilitada { get; set; }

    public virtual Restaurante Restaurante { get; set; } = null!;

    public virtual ICollection<TurnoFila> TurnoFilas { get; set; } = new List<TurnoFila>();
}
