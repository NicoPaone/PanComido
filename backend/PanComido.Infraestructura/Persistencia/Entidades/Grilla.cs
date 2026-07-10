using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("grilla")]
public partial class Grilla
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("restaurante_id")]
    public int RestauranteId { get; set; }

    [Column("cant_columnas")]
    public int CantColumnas { get; set; }

    [Column("cant_filas")]
    public int CantFilas { get; set; }

    [InverseProperty("Grilla")]
    public virtual ICollection<Mesa> Mesas { get; set; } = new List<Mesa>();

    [ForeignKey("RestauranteId")]
    [InverseProperty("Grillas")]
    public virtual Restaurante Restaurante { get; set; } = null!;
}
