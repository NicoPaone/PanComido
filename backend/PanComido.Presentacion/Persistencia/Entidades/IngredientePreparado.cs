using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class IngredientePreparado
{
    public int IdIngrediente { get; set; }

    public virtual Ingrediente IdIngredienteNavigation { get; set; } = null!;

    public virtual ICollection<IngredienteIngredientePreparado> IngredienteIngredientePreparados { get; set; } = new List<IngredienteIngredientePreparado>();
}
