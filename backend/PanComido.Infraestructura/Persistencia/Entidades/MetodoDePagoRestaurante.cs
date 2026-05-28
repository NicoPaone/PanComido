using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[PrimaryKey("RestauranteId", "MetodoDePagoId")]
[Table("metodo_de_pago_restaurante")]
public partial class MetodoDePagoRestaurante
{
    [Key]
    [Column("restaurante_id")]
    public int RestauranteId { get; set; }

    [Key]
    [Column("metodo_de_pago_id")]
    public int MetodoDePagoId { get; set; }

    [Column("habilitado")]
    public bool Habilitado { get; set; }

    [ForeignKey("MetodoDePagoId")]
    [InverseProperty("MetodoDePagoRestaurantes")]
    public virtual MetodoDePago MetodoDePago { get; set; } = null!;

    [ForeignKey("RestauranteId")]
    [InverseProperty("MetodoDePagoRestaurantes")]
    public virtual Restaurante Restaurante { get; set; } = null!;
}
