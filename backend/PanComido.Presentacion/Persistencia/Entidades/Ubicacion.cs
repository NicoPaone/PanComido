using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Ubicacion
{
    public int Id { get; set; }

    public string Direccion { get; set; } = null!;

    public string Ciudad { get; set; } = null!;

    public string CodigoPostal { get; set; } = null!;

    public virtual ICollection<Restaurante> Restaurantes { get; set; } = new List<Restaurante>();
}
