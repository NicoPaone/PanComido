using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Cierre
{
    public int Id { get; set; }

    public int RestauranteId { get; set; }

    public int TurnoLaboralId { get; set; }

    public decimal Diferencia { get; set; }

    public decimal Sobrante { get; set; }

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual Restaurante Restaurante { get; set; } = null!;

    public virtual TurnoLaboral TurnoLaboral { get; set; } = null!;
}
