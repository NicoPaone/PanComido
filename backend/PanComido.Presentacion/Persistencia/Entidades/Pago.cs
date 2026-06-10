using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Pago
{
    public int Id { get; set; }

    public int? CierreId { get; set; }

    public int MetodoPagoId { get; set; }

    public decimal Total { get; set; }

    public virtual Cierre? Cierre { get; set; }

    public virtual ICollection<Comandum> Comanda { get; set; } = new List<Comandum>();

    public virtual MetodoDePago MetodoPago { get; set; } = null!;
}
