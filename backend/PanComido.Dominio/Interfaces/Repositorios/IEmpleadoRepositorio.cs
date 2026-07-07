
using System.Collections.Generic;
using System.Threading.Tasks;
using PanComido.Dominio.Entidades;

namespace PanComido.Dominio.Interfaces.Repositorios
{
   public interface IEmpleadoRepositorio
   {
      Task<Empleado?> ObtenerPorEmailAsync(string email);
      Task<string?> ObtenerRolAsync(int empleadoId);
      Task<List<Empleado>> ObtenerTodosPorRestauranteAsync(int restauranteId);
      Task<Empleado?> ObtenerPorIdYRestauranteAsync(int id, int restauranteId);
      Task CrearAsync(Empleado empleado, List<int> turnosIds);
      Task ModificarAsync(Empleado empleado, List<int> turnosIds);
      Task EliminarLogicoAsync(int id, int restauranteId);
   }
}
