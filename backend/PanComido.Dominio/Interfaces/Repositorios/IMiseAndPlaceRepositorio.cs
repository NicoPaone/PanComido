using PanComido.Dominio.Entidades;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IMiseAndPlaceRepositorio
    {
        Task<int> CrearMiseAndPlaceAsync(NuevoMiseAndPlace nuevoMiseAndPlace, string nombreLote);
        Task<List<MiseAndPlaceListadoDominio>> ObtenerTodosAsync(int restauranteId);
        Task<MiseAndPlaceListadoDominio> ObtenerPorIdAsync(int restauranteId, int miseAndPlaceId);
        Task<bool> EliminarMiseAndPlaceAsync(int restauranteId, int miseAndPlaceId);
        Task<bool> ModificarMiseAndPlaceAsync(int restauranteId, int miseAndPlaceId, ModificarMiseAndPlaceDominio datos);
        Task<bool> ExisteInsumoEnMiseAndPlaceActivosAsync(int insumoId);
    }
}
