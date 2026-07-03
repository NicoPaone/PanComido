using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("encuesta_satisfaccion")]
public partial class EncuestaSatisfaccion
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("comanda_id")]
    public int ComandaId { get; set; }

    [Column("puntuacion_lugar")]
    public int PuntuacionLugar { get; set; }

    [Column("puntuacion_comida")]
    public int PuntuacionComida { get; set; }

    [Column("puntuacion_mozo")]
    public int PuntuacionMozo { get; set; }

    [Column("fecha", TypeName = "timestamp without time zone")]
    public DateTime Fecha { get; set; }

    [ForeignKey("ComandaId")]
    [InverseProperty("EncuestaSatisfaccions")]
    public virtual Comandum Comanda { get; set; } = null!;
}
