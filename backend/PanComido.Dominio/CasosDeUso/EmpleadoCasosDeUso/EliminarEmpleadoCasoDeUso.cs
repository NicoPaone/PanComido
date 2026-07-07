using System.Collections.Generic;
using System.Threading.Tasks;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Dominio.CasosDeUso.EmpleadoCasosDeUso
{
    public class EliminarEmpleadoCasoDeUso
    {
        private readonly IEmpleadoRepositorio _repositorio;

        public EliminarEmpleadoCasoDeUso(IEmpleadoRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task EjecutarAsync(int id, int restauranteId)
        {
            var empleadoExistente = await _repositorio.ObtenerPorIdYRestauranteAsync(id, restauranteId);
            if (empleadoExistente == null)
                throw new KeyNotFoundException("Empleado no encontrado.");

            await _repositorio.EliminarLogicoAsync(id, restauranteId);
        }
    }
}
