using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class TurnoLaboral
{
    public int Id { get; set; }

    public int RestauranteId { get; set; }

    public TimeOnly HorarioLaboralInicio { get; set; }

    public TimeOnly HorarioLaboralFin { get; set; }

    public virtual ICollection<Cierre> Cierres { get; set; } = new List<Cierre>();

    public virtual Restaurante Restaurante { get; set; } = null!;

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
