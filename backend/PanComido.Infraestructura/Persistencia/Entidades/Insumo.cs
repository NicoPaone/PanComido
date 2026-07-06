using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("insumo")]
public partial class Insumo
{
    [Key]
    [Column("id_articulo")]
    public int IdArticulo { get; set; }

    [Column("categoria_insumo_id")]
    public int CategoriaInsumoId { get; set; }

    [Column("unidad_medida_id")]
    public int UnidadMedidaId { get; set; }

    [Column("stock_minimo")]
    public decimal StockMinimo { get; set; }

    [Column("stock_recomendado")]
    public decimal StockRecomendado { get; set; }

    [ForeignKey("CategoriaInsumoId")]
    [InverseProperty("Insumos")]
    public virtual CategoriaInsumo CategoriaInsumo { get; set; } = null!;

    [ForeignKey("IdArticulo")]
    [InverseProperty("Insumo")]
    public virtual Articulo IdArticuloNavigation { get; set; } = null!;

    [InverseProperty("IdInsumoNavigation")]
    public virtual Ingrediente? Ingrediente { get; set; }

    [InverseProperty("Insumo")]
    public virtual ICollection<Lote> Lotes { get; set; } = new List<Lote>();

    [InverseProperty("Insumo")]
    public virtual ICollection<PedidoInsumo> PedidoInsumos { get; set; } = new List<PedidoInsumo>();

    [ForeignKey("UnidadMedidaId")]
    [InverseProperty("Insumos")]
    public virtual UnidadMedidum UnidadMedida { get; set; } = null!;
}
