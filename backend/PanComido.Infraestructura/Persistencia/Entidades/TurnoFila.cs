using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("turno_fila")]
public partial class TurnoFila
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("fila_virtual_id")]
    public int FilaVirtualId { get; set; }

    [Column("numero")]
    public int Numero { get; set; }

    [Column("cantidad_comensales")]
    public int CantidadComensales { get; set; }

    [Column("fecha_hora_ingreso")]
    public DateTime FechaHoraIngreso { get; set; }

    [Column("estado_turno_mesa_id")]
    public int EstadoTurnoMesaId { get; set; }

    [Column("mesa_asignada_id")]
    public int? MesaAsignadaId { get; set; }

    [Column("fecha_hora_asignacion")]
    public DateTime? FechaHoraAsignacion { get; set; }

    [Column("comanda_pre_armada_id")]
    public int? ComandaPreArmadaId { get; set; }

    [ForeignKey("FilaVirtualId")]
    [InverseProperty("TurnoFilas")]
    public virtual FilaVirtual FilaVirtual { get; set; } = null!;
}
