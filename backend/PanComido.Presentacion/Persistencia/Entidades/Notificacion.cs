using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Notificacion
{
    public int Id { get; set; }

    public int RestauranteId { get; set; }

    public DateTime Fecha { get; set; }

    public string Descripcion { get; set; } = null!;

    public bool Resuelta { get; set; }

    public virtual Restaurante Restaurante { get; set; } = null!;
}
