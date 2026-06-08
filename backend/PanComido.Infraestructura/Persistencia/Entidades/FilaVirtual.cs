using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("fila_virtual")]
public partial class FilaVirtual
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("restaurante_id")]
    public int RestauranteId { get; set; }

    [Column("habilitada")]
    public bool Habilitada { get; set; }

    [ForeignKey("RestauranteId")]
    [InverseProperty("FilaVirtuals")]
    public virtual Restaurante Restaurante { get; set; } = null!;

    [InverseProperty("FilaVirtual")]
    public virtual ICollection<TurnoFila> TurnoFilas { get; set; } = new List<TurnoFila>();
}
