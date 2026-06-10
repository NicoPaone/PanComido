using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Mesa
{
    public int Id { get; set; }

    public int GrillaId { get; set; }

    public int EstadoMesaId { get; set; }

    public int DimensionMesaId { get; set; }

    public int PosicionXInicio { get; set; }

    public int PosicionXFin { get; set; }

    public int PosicionYInicio { get; set; }

    public int PosicionYFin { get; set; }

    public int Numero { get; set; }

    public string? CodigoInvitacion { get; set; }

    public int CantPersonasMax { get; set; }

    public virtual ICollection<Comandum> Comanda { get; set; } = new List<Comandum>();

    public virtual DimensionMesa DimensionMesa { get; set; } = null!;

    public virtual EstadoMesa EstadoMesa { get; set; } = null!;

    public virtual Grilla Grilla { get; set; } = null!;

    public virtual ICollection<Llamado> Llamados { get; set; } = new List<Llamado>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<Mozo> Mozos { get; set; } = new List<Mozo>();
}
