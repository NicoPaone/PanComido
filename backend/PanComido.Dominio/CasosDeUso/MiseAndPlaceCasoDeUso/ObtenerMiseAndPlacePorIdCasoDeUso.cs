using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MiseAndPlaceCasoDeUso
{
    public class ObtenerMiseAndPlacePorIdCasoDeUso
    {
        private readonly IMiseAndPlaceRepositorio _miseAndPlaceRepositorio;

        public ObtenerMiseAndPlacePorIdCasoDeUso(IMiseAndPlaceRepositorio miseAndPlaceRepositorio)
        {
            _miseAndPlaceRepositorio = miseAndPlaceRepositorio;
        }

        public async Task<MiseAndPlaceListadoDominio> EjecutarAsync(int restauranteId, int miseAndPlaceId)
        {
            return await _miseAndPlaceRepositorio.ObtenerPorIdAsync(restauranteId, miseAndPlaceId);
        }
    }
}
