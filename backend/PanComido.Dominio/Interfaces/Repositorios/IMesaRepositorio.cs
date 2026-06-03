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
        Task<List<int>> ObtenerMozoIdsPorMesaAsync(int mesaId);
    }
}
