using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("bebida")]
public partial class Bebidum
{
    [Key]
    [Column("id_insumo")]
    public int IdInsumo { get; set; }

    [Column("categoria_bebida_id")]
    public int CategoriaBebidaId { get; set; }

    [ForeignKey("CategoriaBebidaId")]
    [InverseProperty("Bebida")]
    public virtual CategoriaBebidum CategoriaBebida { get; set; } = null!;

    [ForeignKey("IdInsumo")]
    [InverseProperty("Bebidum")]
    public virtual Insumo IdInsumoNavigation { get; set; } = null!;
}
