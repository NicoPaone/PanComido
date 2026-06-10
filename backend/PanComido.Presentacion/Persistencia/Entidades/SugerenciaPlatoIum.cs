using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class SugerenciaPlatoIum
{
    public int Id { get; set; }

    public int RestauranteId { get; set; }

    public string Json { get; set; } = null!;

    public virtual Restaurante Restaurante { get; set; } = null!;
}
