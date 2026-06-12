using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Bodega
{
    public int Id { get; set; }

    public int RestauranteId { get; set; }

    public int TipoBodegaId { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Eliminado { get; set; }

    public virtual ICollection<Lote> Lotes { get; set; } = new List<Lote>();

    public virtual Restaurante Restaurante { get; set; } = null!;

    public virtual TipoBodega TipoBodega { get; set; } = null!;
}
