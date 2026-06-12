using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Entidades;

[Table("restaurante")]
public partial class Restaurante
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("direccion_id")]
    public int DireccionId { get; set; }

    [Column("familia_tipografica_id")]
    public int? FamiliaTipograficaId { get; set; }

    [Column("nombre")]
    public string Nombre { get; set; } = null!;

    [Column("imagen")]
    public string? Imagen { get; set; }

    [Column("color_principal")]
    public string? ColorPrincipal { get; set; }

    [Column("color_secundario")]
    public string? ColorSecundario { get; set; }

    [InverseProperty("Restaurante")]
    public virtual ICollection<Articulo> Articulos { get; set; } = new List<Articulo>();

    [InverseProperty("Restaurante")]
    public virtual ICollection<Bodega> Bodegas { get; set; } = new List<Bodega>();

    [InverseProperty("Restaurante")]
    public virtual ICollection<Cartum> Carta { get; set; } = new List<Cartum>();

    [InverseProperty("Restaurante")]
    public virtual ICollection<Cierre> Cierres { get; set; } = new List<Cierre>();

    [InverseProperty("Restaurante")]
    public virtual ICollection<Comandum> Comanda { get; set; } = new List<Comandum>();

    [ForeignKey("DireccionId")]
    [InverseProperty("Restaurantes")]
    public virtual Ubicacion Direccion { get; set; } = null!;

    [InverseProperty("Restaurante")]
    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();

    [ForeignKey("FamiliaTipograficaId")]
    [InverseProperty("Restaurantes")]
    public virtual FamiliaTipografica? FamiliaTipografica { get; set; }

    [InverseProperty("Restaurante")]
    public virtual ICollection<FilaVirtual> FilaVirtuals { get; set; } = new List<FilaVirtual>();

    [InverseProperty("Restaurante")]
    public virtual ICollection<Grilla> Grillas { get; set; } = new List<Grilla>();

    [InverseProperty("Restaurante")]
    public virtual ICollection<MetodoDePagoRestaurante> MetodoDePagoRestaurantes { get; set; } = new List<MetodoDePagoRestaurante>();

    [InverseProperty("Restaurante")]
    public virtual ICollection<Notificacion> Notificacions { get; set; } = new List<Notificacion>();

    [InverseProperty("Restaurante")]
    public virtual ICollection<PorcentajeCategoriaBebidum> PorcentajeCategoriaBebida { get; set; } = new List<PorcentajeCategoriaBebidum>();

    [InverseProperty("Restaurante")]
    public virtual ICollection<PorcentajeCategoriaPlato> PorcentajeCategoriaPlatos { get; set; } = new List<PorcentajeCategoriaPlato>();

    [InverseProperty("Restaurante")]
    public virtual ICollection<Proveedor> Proveedors { get; set; } = new List<Proveedor>();

    [InverseProperty("Restaurante")]
    public virtual ICollection<SugerenciaPlatoIum> SugerenciaPlatoIa { get; set; } = new List<SugerenciaPlatoIum>();

    [InverseProperty("Restaurante")]
    public virtual ICollection<TurnoLaboral> TurnoLaborals { get; set; } = new List<TurnoLaboral>();
}
