using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Mozo
{
    public int IdEmpleado { get; set; }

    public bool Activo { get; set; }

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    public virtual ICollection<Llamado> Llamados { get; set; } = new List<Llamado>();

    public virtual ICollection<Mesa> Mesas { get; set; } = new List<Mesa>();
}
