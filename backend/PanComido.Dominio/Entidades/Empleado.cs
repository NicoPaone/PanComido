
using System.Collections.Generic;

namespace PanComido.Dominio.Entidades
{
   public class Empleado
   {
      public int Id { get; set; }
      public int RestauranteId { get; set; }
      public string Nombre { get; set; } = string.Empty;
      public string Email { get; set; } = string.Empty;
      public string ContraseniaHash { get; set; } = string.Empty;
      public string Estado { get; set; } = string.Empty;
      public string Rol { get; set; } = string.Empty;
      public List<TurnoLaboral> Turnos { get; set; } = new();
   }
}
