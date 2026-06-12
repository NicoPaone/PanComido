using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("cierre")]
public partial class Cierre
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("restaurante_id")]
    public int RestauranteId { get; set; }

    [Column("turno_laboral_id")]
    public int TurnoLaboralId { get; set; }

    [Column("diferencia")]
    public decimal Diferencia { get; set; }

    [Column("sobrante")]
    public decimal Sobrante { get; set; }

    [Column("total_efectivo")]
    public decimal TotalEfectivo { get; set; }

    [Column("total_tarjeta")]
    public decimal TotalTarjeta { get; set; }

    [Column("total_transferencia")]
    public decimal TotalTransferencia { get; set; }

    [Column("total_mercado_pago")]
    public decimal TotalMercadoPago { get; set; }

    [InverseProperty("Cierre")]
    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    [ForeignKey("RestauranteId")]
    [InverseProperty("Cierres")]
    public virtual Restaurante Restaurante { get; set; } = null!;

    [ForeignKey("TurnoLaboralId")]
    [InverseProperty("Cierres")]
    public virtual TurnoLaboral TurnoLaboral { get; set; } = null!;
}
