using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MiseAndPlaceCasoDeUso
{
    public class ObtenerTodosLosMiseAndPlaceCasoDeUso
    {
        private readonly IMiseAndPlaceRepositorio _miseAndPlaceRepositorio;

        public ObtenerTodosLosMiseAndPlaceCasoDeUso(IMiseAndPlaceRepositorio miseAndPlaceRepositorio)
        {
            _miseAndPlaceRepositorio = miseAndPlaceRepositorio;
        }

        public async Task<List<MiseAndPlaceListadoDominio>> EjecutarAsync(int restauranteId)
        {
            return await _miseAndPlaceRepositorio.ObtenerTodosAsync(restauranteId);
        }
    }
}
