using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("pago")]
public partial class Pago
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("comanda_id")]
    public int ComandaId { get; set; }

    [Column("cierre_id")]
    public int? CierreId { get; set; }

    [Column("metodo_pago_id")]
    public int MetodoPagoId { get; set; }

    [Column("estado_pago_id")]
    public int EstadoPagoId { get; set; }

    [Column("external_reference")]
    public string? ExternalReference { get; set; }

    [Column("total")]
    public decimal Total { get; set; }

    [Column("fecha_hora", TypeName = "timestamp without time zone")]
    public DateTime FechaHora { get; set; }

    [ForeignKey("CierreId")]
    [InverseProperty("Pagos")]
    public virtual Cierre? Cierre { get; set; }

    [ForeignKey("ComandaId")]
    [InverseProperty("Pagos")]
    public virtual Comandum Comanda { get; set; } = null!;

    [ForeignKey("EstadoPagoId")]
    [InverseProperty("Pagos")]
    public virtual EstadoPago EstadoPago { get; set; } = null!;

    [ForeignKey("MetodoPagoId")]
    [InverseProperty("Pagos")]
    public virtual MetodoDePago MetodoPago { get; set; } = null!;
}
