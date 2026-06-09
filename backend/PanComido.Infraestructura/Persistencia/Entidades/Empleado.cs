using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("empleado")]
public partial class Empleado
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("restaurante_id")]
    public int RestauranteId { get; set; }

    [Column("nombre")]
    public string Nombre { get; set; } = null!;

    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("contrasena")]
    public string Contrasena { get; set; } = null!;

    [Column("estado")]
    public string Estado { get; set; } = null!;

    [Column("eliminado")]
    public bool Eliminado { get; set; }

    [InverseProperty("IdEmpleadoNavigation")]
    public virtual Cocina? Cocina { get; set; }

    [InverseProperty("IdEmpleadoNavigation")]
    public virtual Gerente? Gerente { get; set; }

    [InverseProperty("IdEmpleadoNavigation")]
    public virtual Mozo? Mozo { get; set; }

    [ForeignKey("RestauranteId")]
    [InverseProperty("Empleados")]
    public virtual Restaurante Restaurante { get; set; } = null!;

    [ForeignKey("EmpleadoId")]
    [InverseProperty("Empleados")]
    public virtual ICollection<TurnoLaboral> TurnoLaborals { get; set; } = new List<TurnoLaboral>();
}
