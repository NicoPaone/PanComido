using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Lote
{
    public int Id { get; set; }

    public int InsumoId { get; set; }

    public int BodegaId { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Cantidad { get; set; }

    public DateOnly FechaAdquisicion { get; set; }

    public DateOnly? FechaVencimiento { get; set; }

    public virtual Bodega Bodega { get; set; } = null!;

    public virtual Insumo Insumo { get; set; } = null!;
}
