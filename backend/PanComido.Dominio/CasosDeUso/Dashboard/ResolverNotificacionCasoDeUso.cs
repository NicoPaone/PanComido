using PanComido.Dominio.Interfaces.Repositorios;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.Dashboard
{
    public class ResolverNotificacionCasoDeUso
    {
        private readonly IPlatoAnalisisRepositorio _platoAnalisisRepositorio;

        public ResolverNotificacionCasoDeUso(IPlatoAnalisisRepositorio platoAnalisisRepositorio)
        {
            _platoAnalisisRepositorio = platoAnalisisRepositorio;
        }

        public async Task EjecutarAsync(int restauranteId, int id)
        {
            await _platoAnalisisRepositorio.ResolverNotificacionAsync(restauranteId, id);
        }
    }
}
