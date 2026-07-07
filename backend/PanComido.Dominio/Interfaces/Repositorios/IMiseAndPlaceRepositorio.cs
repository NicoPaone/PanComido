using PanComido.Dominio.Entidades;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IMiseAndPlaceRepositorio
    {
        Task<int> CrearMiseAndPlaceAsync(NuevoMiseAndPlace nuevoMiseAndPlace, string nombreLote);
        Task<List<MiseAndPlaceListadoDominio>> ObtenerTodosAsync(int restauranteId);
        Task<MiseAndPlaceListadoDominio> ObtenerPorIdAsync(int restauranteId, int miseAndPlaceId);
    }
}
