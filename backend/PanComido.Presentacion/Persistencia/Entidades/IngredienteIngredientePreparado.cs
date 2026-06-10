using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class IngredienteIngredientePreparado
{
    public int IngredienteId { get; set; }

    public int IngredientePreparadoId { get; set; }

    public decimal Cantidad { get; set; }

    public virtual Ingrediente Ingrediente { get; set; } = null!;

    public virtual IngredientePreparado IngredientePreparado { get; set; } = null!;
}
