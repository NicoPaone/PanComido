using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("turno_laboral")]
public partial class TurnoLaboral
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("restaurante_id")]
    public int RestauranteId { get; set; }

    [Column("horario_laboral_inicio")]
    public TimeOnly HorarioLaboralInicio { get; set; }

    [Column("horario_laboral_fin")]
    public TimeOnly HorarioLaboralFin { get; set; }

    [Column("es_nocturno")]
    public bool EsNocturno { get; set; }

    [InverseProperty("TurnoLaboral")]
    public virtual ICollection<Cierre> Cierres { get; set; } = new List<Cierre>();

    [ForeignKey("RestauranteId")]
    [InverseProperty("TurnoLaborals")]
    public virtual Restaurante Restaurante { get; set; } = null!;

    [ForeignKey("TurnoLaboralId")]
    [InverseProperty("TurnoLaborals")]
    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
