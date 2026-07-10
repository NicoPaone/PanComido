using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("ubicacion")]
public partial class Ubicacion
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("direccion")]
    public string Direccion { get; set; } = null!;

    [Column("ciudad")]
    public string Ciudad { get; set; } = null!;

    [Column("codigo_postal")]
    public string CodigoPostal { get; set; } = null!;

    [InverseProperty("Direccion")]
    public virtual ICollection<Restaurante> Restaurantes { get; set; } = new List<Restaurante>();
}
