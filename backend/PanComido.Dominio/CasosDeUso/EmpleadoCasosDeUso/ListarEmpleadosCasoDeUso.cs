using System.Collections.Generic;
using System.Threading.Tasks;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Dominio.CasosDeUso.EmpleadoCasosDeUso
{
    public class ListarEmpleadosCasoDeUso
    {
        private readonly IEmpleadoRepositorio _repositorio;

        public ListarEmpleadosCasoDeUso(IEmpleadoRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<Empleado>> EjecutarAsync(int restauranteId)
        {
            return await _repositorio.ObtenerTodosPorRestauranteAsync(restauranteId);
        }
    }
}
