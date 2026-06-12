using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Llamado
{
    public int Id { get; set; }

    public int? MozoId { get; set; }

    public int? GerenteId { get; set; }

    public int? MesaId { get; set; }

    public int CategoriaLlamadoId { get; set; }

    public string? Descripcion { get; set; }

    public bool Resuelto { get; set; }

    public virtual CategoriaLlamado CategoriaLlamado { get; set; } = null!;

    public virtual Gerente? Gerente { get; set; }

    public virtual Mesa? Mesa { get; set; }

    public virtual Mozo? Mozo { get; set; }
}
