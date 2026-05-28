using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("cocina")]
public partial class Cocina
{
    [Key]
    [Column("id_empleado")]
    public int IdEmpleado { get; set; }

    [ForeignKey("IdEmpleado")]
    [InverseProperty("Cocina")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}
