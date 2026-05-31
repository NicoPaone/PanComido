using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("metodo_de_pago")]
[Index("Descripcion", Name = "metodo_de_pago_descripcion_key", IsUnique = true)]
public partial class MetodoDePago
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; } = null!;

    [InverseProperty("MetodoDePago")]
    public virtual ICollection<MetodoDePagoRestaurante> MetodoDePagoRestaurantes { get; set; } = new List<MetodoDePagoRestaurante>();

    [InverseProperty("MetodoPago")]
    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
