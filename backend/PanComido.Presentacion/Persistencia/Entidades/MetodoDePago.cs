using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class MetodoDePago
{
    public int Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<MetodoDePagoRestaurante> MetodoDePagoRestaurantes { get; set; } = new List<MetodoDePagoRestaurante>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
