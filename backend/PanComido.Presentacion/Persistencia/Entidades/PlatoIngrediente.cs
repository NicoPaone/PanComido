using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class PlatoIngrediente
{
    public int PlatoId { get; set; }

    public int IngredienteId { get; set; }

    public bool Opcional { get; set; }

    public decimal Cantidad { get; set; }

    public virtual Ingrediente Ingrediente { get; set; } = null!;

    public virtual Plato Plato { get; set; } = null!;
}
