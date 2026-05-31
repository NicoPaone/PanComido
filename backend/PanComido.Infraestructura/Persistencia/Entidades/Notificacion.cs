using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("notificacion")]
public partial class Notificacion
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("restaurante_id")]
    public int RestauranteId { get; set; }

    [Column("fecha", TypeName = "timestamp without time zone")]
    public DateTime Fecha { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; } = null!;

    [Column("resuelta")]
    public bool Resuelta { get; set; }

    [ForeignKey("RestauranteId")]
    [InverseProperty("Notificacions")]
    public virtual Restaurante Restaurante { get; set; } = null!;
}
