using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Cartum
{
    public int Id { get; set; }

    public int RestauranteId { get; set; }

    public virtual ICollection<Articulo> Articulos { get; set; } = new List<Articulo>();

    public virtual Restaurante Restaurante { get; set; } = null!;
}
