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

    [Column("stock_minimo")]
    public decimal StockMinimo { get; set; }

    [Column("vencimiento")]
    public DateOnly? Vencimiento { get; set; }

    [InverseProperty("IdInsumoNavigation")]
    public virtual Bebidum? Bebidum { get; set; }

    [ForeignKey("IdArticulo")]
    [InverseProperty("Insumo")]
    public virtual Articulo IdArticuloNavigation { get; set; } = null!;

    [InverseProperty("IdInsumoNavigation")]
    public virtual Ingrediente? Ingrediente { get; set; }

    [InverseProperty("Insumo")]
    public virtual ICollection<Lote> Lotes { get; set; } = new List<Lote>();

    [InverseProperty("Insumo")]
    public virtual ICollection<PedidoInsumo> PedidoInsumos { get; set; } = new List<PedidoInsumo>();
}
