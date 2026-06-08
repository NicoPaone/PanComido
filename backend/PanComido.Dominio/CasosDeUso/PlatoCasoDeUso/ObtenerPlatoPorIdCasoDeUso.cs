using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.PlatoCasosDeUso
{
    public class ObtenerPlatoPorIdCasoDeUso
    {
        private readonly IPlatoRepositorio _platoRepositorio;

        public ObtenerPlatoPorIdCasoDeUso(IPlatoRepositorio platoRepositorio)
        {
            _platoRepositorio = platoRepositorio;
        }

        public async Task<Plato> EjecutarAsync(int id, int restauranteId)
        {
            return await _platoRepositorio.ObtenerPorIdAsync(id, restauranteId);
        }
    }
}
