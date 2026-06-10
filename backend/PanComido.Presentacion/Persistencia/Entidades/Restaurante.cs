using System;
using System.Collections.Generic;

namespace PanComido.Presentacion.Persistencia.Entidades;

public partial class Restaurante
{
    public int Id { get; set; }

    public int DireccionId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Imagen { get; set; }

    public string? ColorPrincipal { get; set; }

    public string? ColorSecundario { get; set; }

    public string? TextoPrincipal { get; set; }

    public string? TextoSecundario { get; set; }

    public virtual ICollection<Articulo> Articulos { get; set; } = new List<Articulo>();

    public virtual ICollection<Bodega> Bodegas { get; set; } = new List<Bodega>();

    public virtual ICollection<Cartum> Carta { get; set; } = new List<Cartum>();

    public virtual ICollection<Cierre> Cierres { get; set; } = new List<Cierre>();

    public virtual ICollection<Comandum> Comanda { get; set; } = new List<Comandum>();

    public virtual Ubicacion Direccion { get; set; } = null!;

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();

    public virtual ICollection<FilaVirtual> FilaVirtuals { get; set; } = new List<FilaVirtual>();

    public virtual ICollection<Grilla> Grillas { get; set; } = new List<Grilla>();

    public virtual ICollection<MetodoDePagoRestaurante> MetodoDePagoRestaurantes { get; set; } = new List<MetodoDePagoRestaurante>();

    public virtual ICollection<Notificacion> Notificacions { get; set; } = new List<Notificacion>();

    public virtual ICollection<Proveedor> Proveedors { get; set; } = new List<Proveedor>();

    public virtual ICollection<SugerenciaPlatoIum> SugerenciaPlatoIa { get; set; } = new List<SugerenciaPlatoIum>();

    public virtual ICollection<TurnoLaboral> TurnoLaborals { get; set; } = new List<TurnoLaboral>();
}
