using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("comanda")]
public partial class Comandum
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("mesa_id")]
    public int MesaId { get; set; }

    [Column("pago_id")]
    public int? PagoId { get; set; }

    [Column("restaurante_id")]
    public int RestauranteId { get; set; }

    [Column("estado_comanda_id")]
    public int EstadoComandaId { get; set; }

    [Column("cant_comensales")]
    public int CantComensales { get; set; }

    [Column("hora_inicio", TypeName = "timestamp without time zone")]
    public DateTime HoraInicio { get; set; }

    [Column("hora_fin", TypeName = "timestamp without time zone")]
    public DateTime? HoraFin { get; set; }

    [Column("hora_ultimo_cambio_estado", TypeName = "timestamp without time zone")]
    public DateTime HoraUltimoCambioEstado { get; set; }

    [InverseProperty("Comanda")]
    public virtual ICollection<ArticuloComandum> ArticuloComanda { get; set; } = new List<ArticuloComandum>();

    [ForeignKey("EstadoComandaId")]
    [InverseProperty("Comanda")]
    public virtual EstadoComandum EstadoComanda { get; set; } = null!;

    [ForeignKey("MesaId")]
    [InverseProperty("Comanda")]
    public virtual Mesa Mesa { get; set; } = null!;

    [ForeignKey("PagoId")]
    [InverseProperty("Comanda")]
    public virtual Pago? Pago { get; set; }

    [ForeignKey("RestauranteId")]
    [InverseProperty("Comanda")]
    public virtual Restaurante Restaurante { get; set; } = null!;
}
