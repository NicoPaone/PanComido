using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("llamado")]
public partial class Llamado
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("mozo_id")]
    public int? MozoId { get; set; }

    [Column("gerente_id")]
    public int? GerenteId { get; set; }

    [Column("mesa_id")]
    public int? MesaId { get; set; }

    [Column("categoria_llamado_id")]
    public int CategoriaLlamadoId { get; set; }

    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [Column("resuelto")]
    public bool Resuelto { get; set; }

    [ForeignKey("CategoriaLlamadoId")]
    [InverseProperty("Llamados")]
    public virtual CategoriaLlamado CategoriaLlamado { get; set; } = null!;

    [ForeignKey("GerenteId")]
    [InverseProperty("Llamados")]
    public virtual Gerente? Gerente { get; set; }

    [ForeignKey("MesaId")]
    [InverseProperty("Llamados")]
    public virtual Mesa? Mesa { get; set; }

    [ForeignKey("MozoId")]
    [InverseProperty("Llamados")]
    public virtual Mozo? Mozo { get; set; }
}
