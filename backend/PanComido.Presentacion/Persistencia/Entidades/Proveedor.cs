using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Proveedor
{
    public int Id { get; set; }

    public int RestauranteId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? NumeroTelefonoWsp { get; set; }

    public bool Eliminado { get; set; }

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    public virtual Restaurante Restaurante { get; set; } = null!;

    public virtual ICollection<CategoriaInsumo> CategoriaInsumos { get; set; } = new List<CategoriaInsumo>();
}
