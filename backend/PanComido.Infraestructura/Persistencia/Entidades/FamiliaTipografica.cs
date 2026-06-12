using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("familia_tipografica")]
public partial class FamiliaTipografica
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("categoria")]
    public string Categoria { get; set; } = null!;

    [Column("tipografia_titulo")]
    public string TipografiaTitulo { get; set; } = null!;

    [Column("tipografia_cuerpo")]
    public string TipografiaCuerpo { get; set; } = null!;

    [InverseProperty("FamiliaTipografica")]
    public virtual ICollection<Restaurante> Restaurantes { get; set; } = new List<Restaurante>();
}
