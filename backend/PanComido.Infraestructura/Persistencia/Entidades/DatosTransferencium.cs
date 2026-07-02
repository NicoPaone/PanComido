using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("datos_transferencia")]
[Index("RestauranteId", Name = "datos_transferencia_restaurante_id_key", IsUnique = true)]
public partial class DatosTransferencium
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("restaurante_id")]
    public int RestauranteId { get; set; }

    [Column("alias")]
    public string Alias { get; set; } = null!;

    [Column("cbu")]
    public string? Cbu { get; set; }

    [Column("numero_cuenta")]
    public string NumeroCuenta { get; set; } = null!;

    [Column("titular_cuenta")]
    public string TitularCuenta { get; set; } = null!;

    [ForeignKey("RestauranteId")]
    [InverseProperty("DatosTransferencium")]
    public virtual Restaurante Restaurante { get; set; } = null!;
}
