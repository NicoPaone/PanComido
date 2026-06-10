using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Reserva
{
    public int Id { get; set; }

    public int MesaId { get; set; }

    public int CantComensales { get; set; }

    public string NombreTitular { get; set; } = null!;

    public DateOnly Fecha { get; set; }

    public TimeOnly Horario { get; set; }

    public string? TelContacto { get; set; }

    public virtual Mesa Mesa { get; set; } = null!;
}
