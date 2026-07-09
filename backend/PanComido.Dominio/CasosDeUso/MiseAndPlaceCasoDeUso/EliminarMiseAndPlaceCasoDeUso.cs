using PanComido.Dominio.Interfaces.Repositorios;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MiseAndPlaceCasoDeUso
{
    public class EliminarMiseAndPlaceCasoDeUso
    {
        private readonly IMiseAndPlaceRepositorio _repositorio;

        public EliminarMiseAndPlaceCasoDeUso(IMiseAndPlaceRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<bool> EjecutarAsync(int restauranteId, int miseAndPlaceId)
        {
            return await _repositorio.EliminarMiseAndPlaceAsync(restauranteId, miseAndPlaceId);
        }
    }
}
