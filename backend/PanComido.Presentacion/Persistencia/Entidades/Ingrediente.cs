using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Ingrediente
{
    public int IdInsumo { get; set; }

    public virtual Insumo IdInsumoNavigation { get; set; } = null!;

    public virtual ICollection<IngredienteIngredientePreparado> IngredienteIngredientePreparados { get; set; } = new List<IngredienteIngredientePreparado>();

    public virtual IngredientePreparado? IngredientePreparado { get; set; }

    public virtual ICollection<PlatoIngrediente> PlatoIngredientes { get; set; } = new List<PlatoIngrediente>();
}
