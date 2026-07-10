using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class ListarMozosParaMesaCasoDeUso
    {
        private readonly IMesaRepositorio _mesaRepositorio;

        public ListarMozosParaMesaCasoDeUso(IMesaRepositorio mesaRepositorio)
        {
            _mesaRepositorio = mesaRepositorio;
        }

        public async Task<List<Empleado>> EjecutarAsync(int restauranteId)
        {
            return await _mesaRepositorio.ObtenerTodosLosMozosAsync(restauranteId);
        }
    }
}
