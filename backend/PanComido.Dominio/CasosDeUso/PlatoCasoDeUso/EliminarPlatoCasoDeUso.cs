using PanComido.Dominio.Interfaces.Repositorios;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.PlatoCasoDeUso
{
    public class EliminarPlatoCasoDeUso
    {
        private readonly IPlatoRepositorio _platoRepositorio;

        public EliminarPlatoCasoDeUso(IPlatoRepositorio platoRepositorio)
        {
            _platoRepositorio = platoRepositorio;
        }

        public async Task EjecutarAsync(int platoId, int restauranteId)
        {
            await _platoRepositorio.EliminarAsync(platoId, restauranteId);
        }
    }
}
