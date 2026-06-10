using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class MetodoDePagoRestaurante
{
    public int RestauranteId { get; set; }

    public int MetodoDePagoId { get; set; }

    public bool Habilitado { get; set; }

    public virtual MetodoDePago MetodoDePago { get; set; } = null!;

    public virtual Restaurante Restaurante { get; set; } = null!;
}
