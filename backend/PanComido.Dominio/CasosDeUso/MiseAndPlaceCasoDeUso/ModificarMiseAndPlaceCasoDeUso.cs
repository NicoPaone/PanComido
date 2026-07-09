using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MiseAndPlaceCasoDeUso
{
    public class ModificarMiseAndPlaceCasoDeUso
    {
        private readonly IMiseAndPlaceRepositorio _repositorio;

        public ModificarMiseAndPlaceCasoDeUso(IMiseAndPlaceRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<bool> EjecutarAsync(int restauranteId, int miseAndPlaceId, ModificarMiseAndPlaceDominio datos)
        {
            return await _repositorio.ModificarMiseAndPlaceAsync(restauranteId, miseAndPlaceId, datos);
        }
    }
}
