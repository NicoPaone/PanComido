using PanComido.Dominio.Entidades;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IFilaVirtualRepositorio
    {
        Task<FilaVirtual?> ObtenerFilaVirtualAsync(int restautanteId);
        Task<FilaVirtual> ActualizarFilaVirtualAsync(int restauranteId, bool habilitada);
    }
}
