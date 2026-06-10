using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Grilla
{
    public int Id { get; set; }

    public int RestauranteId { get; set; }

    public int CantColumnas { get; set; }

    public int CantFilas { get; set; }

    public virtual ICollection<Mesa> Mesas { get; set; } = new List<Mesa>();

    public virtual Restaurante Restaurante { get; set; } = null!;
}
