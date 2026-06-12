using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Empleado
{
    public int Id { get; set; }

    public int RestauranteId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public bool Eliminado { get; set; }

    public virtual Cocina? Cocina { get; set; }

    public virtual Gerente? Gerente { get; set; }

    public virtual Mozo? Mozo { get; set; }

    public virtual Restaurante Restaurante { get; set; } = null!;

    public virtual ICollection<TurnoLaboral> TurnoLaborals { get; set; } = new List<TurnoLaboral>();
}
