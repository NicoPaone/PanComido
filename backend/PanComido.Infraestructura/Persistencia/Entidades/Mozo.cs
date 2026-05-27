using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("mozo")]
public partial class Mozo
{
    [Key]
    [Column("id_empleado")]
    public int IdEmpleado { get; set; }

    [Column("activo")]
    public bool Activo { get; set; }

    [ForeignKey("IdEmpleado")]
    [InverseProperty("Mozo")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    [InverseProperty("Mozo")]
    public virtual ICollection<Llamado> Llamados { get; set; } = new List<Llamado>();

    [ForeignKey("MozoId")]
    [InverseProperty("Mozos")]
    public virtual ICollection<Mesa> Mesas { get; set; } = new List<Mesa>();
}
