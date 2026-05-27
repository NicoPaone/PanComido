using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("reserva")]
public partial class Reserva
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("mesa_id")]
    public int MesaId { get; set; }

    [Column("cant_comensales")]
    public int CantComensales { get; set; }

    [Column("nombre_titular")]
    public string NombreTitular { get; set; } = null!;

    [Column("fecha")]
    public DateOnly Fecha { get; set; }

    [Column("horario")]
    public TimeOnly Horario { get; set; }

    [Column("tel_contacto")]
    public string? TelContacto { get; set; }

    [ForeignKey("MesaId")]
    [InverseProperty("Reservas")]
    public virtual Mesa Mesa { get; set; } = null!;
}
