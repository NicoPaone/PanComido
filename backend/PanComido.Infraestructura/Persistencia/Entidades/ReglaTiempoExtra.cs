using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("regla_tiempo_extra")]
public partial class ReglaTiempoExtra
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("restaurante_id")]
    public int RestauranteId { get; set; }

    [Column("porcentaje_ocupacion_hasta")]
    public int PorcentajeOcupacionHasta { get; set; }

    [Column("minutos_extra")]
    public int MinutosExtra { get; set; }

    [ForeignKey("RestauranteId")]
    [InverseProperty("ReglaTiempoExtras")]
    public virtual Restaurante Restaurante { get; set; } = null!;
}
