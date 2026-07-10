using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("articulo_comanda_ingrediente_excluido")]
[Index("ArticuloComandaId", "IngredienteId", Name = "articulo_comanda_ingrediente__articulo_comanda_id_ingredien_key", IsUnique = true)]
public partial class ArticuloComandaIngredienteExcluido
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("articulo_comanda_id")]
    public int ArticuloComandaId { get; set; }

    [Column("ingrediente_id")]
    public int IngredienteId { get; set; }

    [ForeignKey("ArticuloComandaId")]
    [InverseProperty("ArticuloComandaIngredienteExcluidos")]
    public virtual ArticuloComandum ArticuloComanda { get; set; } = null!;

    [ForeignKey("IngredienteId")]
    [InverseProperty("ArticuloComandaIngredienteExcluidos")]
    public virtual Ingrediente Ingrediente { get; set; } = null!;
}
