using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("turno_fila")]
public partial class TurnoFila
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("fila_virtual_id")]
    public int FilaVirtualId { get; set; }

    [Column("numero")]
    public int Numero { get; set; }

    [ForeignKey("FilaVirtualId")]
    [InverseProperty("TurnoFilas")]
    public virtual FilaVirtual FilaVirtual { get; set; } = null!;
}
