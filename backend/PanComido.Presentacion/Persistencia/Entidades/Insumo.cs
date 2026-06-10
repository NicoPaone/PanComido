using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Insumo
{
    public int IdArticulo { get; set; }

    public int CategoriaInsumoId { get; set; }

    public int UnidadMedidaId { get; set; }

    public decimal StockMinimo { get; set; }

    public virtual CategoriaInsumo CategoriaInsumo { get; set; } = null!;

    public virtual Articulo IdArticuloNavigation { get; set; } = null!;

    public virtual Ingrediente? Ingrediente { get; set; }

    public virtual ICollection<Lote> Lotes { get; set; } = new List<Lote>();

    public virtual ICollection<PedidoInsumo> PedidoInsumos { get; set; } = new List<PedidoInsumo>();

    public virtual UnidadMedidum UnidadMedida { get; set; } = null!;
}
