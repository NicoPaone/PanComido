using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("gerente")]
public partial class Gerente
{
    [Key]
    [Column("id_empleado")]
    public int IdEmpleado { get; set; }

    [ForeignKey("IdEmpleado")]
    [InverseProperty("Gerente")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    [InverseProperty("Gerente")]
    public virtual ICollection<Llamado> Llamados { get; set; } = new List<Llamado>();
}
