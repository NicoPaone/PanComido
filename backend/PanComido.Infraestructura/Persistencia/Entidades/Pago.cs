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

    [Column("cierre_id")]
    public int CierreId { get; set; }

    [Column("metodo_pago_id")]
    public int MetodoPagoId { get; set; }

    [Column("total")]
    public decimal Total { get; set; }

    [ForeignKey("CierreId")]
    [InverseProperty("Pagos")]
    public virtual Cierre Cierre { get; set; } = null!;

    [InverseProperty("Pago")]
    public virtual ICollection<Comandum> Comanda { get; set; } = new List<Comandum>();

    [ForeignKey("MetodoPagoId")]
    [InverseProperty("Pagos")]
    public virtual MetodoDePago MetodoPago { get; set; } = null!;
}
