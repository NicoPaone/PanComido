using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("proveedor")]
public partial class Proveedor
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("restaurante_id")]
    public int RestauranteId { get; set; }

    [Column("nombre")]
    public string Nombre { get; set; } = null!;

    [Column("numero_telefono_wsp")]
    public string? NumeroTelefonoWsp { get; set; }

    [Column("eliminado")]
    public bool Eliminado { get; set; }

    [InverseProperty("Proveedor")]
    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    [ForeignKey("RestauranteId")]
    [InverseProperty("Proveedors")]
    public virtual Restaurante Restaurante { get; set; } = null!;

    [ForeignKey("ProveedorId")]
    [InverseProperty("Proveedors")]
    public virtual ICollection<CategoriaInsumo> CategoriaInsumos { get; set; } = new List<CategoriaInsumo>();
}
