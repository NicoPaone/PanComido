using PanComido.Dominio.Entidades;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface ITipoBodegaRepositorio
    {
        Task<List<TipoBodega>> ObtenerTodosAsync();
        Task<bool> ExisteAsync(int id);
    }
}
