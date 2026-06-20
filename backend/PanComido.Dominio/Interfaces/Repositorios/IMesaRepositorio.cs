using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Task AsignarMozosAsync(int restauranteId, int mesaId, List<int> mozosIds);
        Task DesasignarMozoAsync(int restauranteId, int mesaId, int mozoId);
        Task GuardarMapaMasivoAsync(int restauranteId, List<MesaMapaDominio> mesas);
        Task<List<Empleado>> ObtenerTodosLosMozosAsync(int restauranteId);
    }
}
