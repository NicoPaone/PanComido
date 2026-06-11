using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Comandum
{
    public int Id { get; set; }

    public int MesaId { get; set; }

    public int? PagoId { get; set; }

    public int RestauranteId { get; set; }

    public int EstadoComandaId { get; set; }

    public int CantComensales { get; set; }

    public DateTime HoraInicio { get; set; }

    public DateTime? HoraFin { get; set; }

    public DateTime HoraUltimoCambioEstado { get; set; }

    public virtual ICollection<ArticuloComandum> ArticuloComanda { get; set; } = new List<ArticuloComandum>();

    public virtual EstadoComandum EstadoComanda { get; set; } = null!;

    public virtual Mesa Mesa { get; set; } = null!;

    public virtual Pago? Pago { get; set; }

    public virtual Restaurante Restaurante { get; set; } = null!;
}
