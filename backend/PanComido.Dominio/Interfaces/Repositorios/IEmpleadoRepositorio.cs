
using PanComido.Dominio.Entidades;

namespace PanComido.Dominio.Interfaces.Repositorios
{
   public interface IEmpleadoRepositorio
   {
      Task<Empleado?> ObtenerPorEmailAsync(string email);
      Task<string?> ObtenerRolAsync(int empleadoId);

   }
}
