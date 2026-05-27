using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("configuracion_articulo")]
[Index("Descripcion", Name = "configuracion_articulo_descripcion_key", IsUnique = true)]
public partial class ConfiguracionArticulo
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; } = null!;

    [ForeignKey("ConfiguracionArticuloId")]
    [InverseProperty("ConfiguracionArticulos")]
    public virtual ICollection<Articulo> Articulos { get; set; } = new List<Articulo>();
}
