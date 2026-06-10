using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class CategoriaInsumo
{
    public int Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public int TipoAplica { get; set; }

    public virtual ICollection<Insumo> Insumos { get; set; } = new List<Insumo>();

    public virtual ICollection<Proveedor> Proveedors { get; set; } = new List<Proveedor>();
}
