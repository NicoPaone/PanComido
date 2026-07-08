using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IMesaRepositorio
    {
        Task<MesaConPosiciones?> ObtenerPorIdAsync(int id, int restauranteId);
        Task ActualizarAsync(Mesa mesa);
        Task ActualizarEstadoAsync(int mesaId, EstadoMesa nuevoEstado);
        Task<List<MesaConPosiciones>> ObtenerTodasAsync(int restauranteId);
        Task<List<MesaConPosiciones>> ObtenerOcupadasAsync(int restauranteId);
        Task<List<MesaConPosiciones>> ObtenerDisponiblesAsync(int restauranteId);
        Task<List<int>> ObtenerMozoIdsPorMesaAsync(int mesaId);
        Task AsignarMozosAsync(int mesaId, List<int> mozosAAgregarIds, List<int> mozosAEliminarIds);
        Task DesasignarMozoAsync(int mesaId, int mozoId);
        Task GuardarMapaMasivoAsync(int restauranteId, List<MesaMapaDominio> mesas);
        Task<List<Empleado>> ObtenerTodosLosMozosAsync(int restauranteId);
        Task<List<int>> ObtenerIdsMesasActivasAsync(int restauranteId);
        Task<bool> TieneComandasActivasAsync(List<int> mesaIds);
        Task<bool> TieneMozosAsignadosAsync(List<int> mesaIds);
    }
}
