using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MiseAndPlaceCasoDeUso
{
    public class EliminarMiseAndPlaceCasoDeUso
    {
        private readonly EliminarInsumoCasoDeUso _eliminarInsumo;

        public EliminarMiseAndPlaceCasoDeUso(EliminarInsumoCasoDeUso eliminarInsumo)
        {
            _eliminarInsumo = eliminarInsumo;
        }

        public async Task<bool> EjecutarAsync(int restauranteId, int miseAndPlaceId)
        {
            try
            {
                await _eliminarInsumo.EjecutarAsync(miseAndPlaceId, restauranteId);
                return true;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }
    }
}
