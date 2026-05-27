using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("carta")]
public partial class Cartum
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("restaurante_id")]
    public int RestauranteId { get; set; }

    [InverseProperty("Carta")]
    public virtual ICollection<Articulo> Articulos { get; set; } = new List<Articulo>();

    [ForeignKey("RestauranteId")]
    [InverseProperty("Carta")]
    public virtual Restaurante Restaurante { get; set; } = null!;
}
