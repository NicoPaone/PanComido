using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("mesa")]
public partial class Mesa
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("grilla_id")]
    public int GrillaId { get; set; }

    [Column("estado_mesa_id")]
    public int EstadoMesaId { get; set; }

    [Column("dimension_mesa_id")]
    public int DimensionMesaId { get; set; }

    [Column("posicion_x_inicio")]
    public int PosicionXInicio { get; set; }

    [Column("posicion_x_fin")]
    public int PosicionXFin { get; set; }

    [Column("posicion_y_inicio")]
    public int PosicionYInicio { get; set; }

    [Column("posicion_y_fin")]
    public int PosicionYFin { get; set; }

    [Column("numero")]
    public int Numero { get; set; }

    [Column("codigo_invitacion")]
    public string? CodigoInvitacion { get; set; }

    [Column("cant_personas_max")]
    public int CantPersonasMax { get; set; }

    [InverseProperty("Mesa")]
    public virtual ICollection<Comandum> Comanda { get; set; } = new List<Comandum>();

    [ForeignKey("DimensionMesaId")]
    [InverseProperty("Mesas")]
    public virtual DimensionMesa DimensionMesa { get; set; } = null!;

    [ForeignKey("EstadoMesaId")]
    [InverseProperty("Mesas")]
    public virtual EstadoMesa EstadoMesa { get; set; } = null!;

    [ForeignKey("GrillaId")]
    [InverseProperty("Mesas")]
    public virtual Grilla Grilla { get; set; } = null!;

    [InverseProperty("Mesa")]
    public virtual ICollection<Llamado> Llamados { get; set; } = new List<Llamado>();

    [InverseProperty("Mesa")]
    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    [ForeignKey("MesaId")]
    [InverseProperty("Mesas")]
    public virtual ICollection<Mozo> Mozos { get; set; } = new List<Mozo>();
}
