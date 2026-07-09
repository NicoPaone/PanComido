using PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IBodegaRepositorio
    {
        Task<bool> ExisteBodegaEnRestauranteAsync(int restauranteId, int bodegaId);
        Task<List<Bodega>> ObtenerBodegasAsync(int restauranteId);
        Task<Bodega> ObtenerBodegaPorIdAsync(int id, int restauranteId);
        Task<Bodega> CrearAsync(Bodega bodega, int restauranteId);
        Task<Bodega> ModificarAsync(Bodega bodega, int restauranteId);
        Task<bool> EliminarAsync(int id, int restauranteId);
        Task<bool> TieneLotesAsociadosAsync(int bodegaId);
        Task<bool> ExisteBodegaPorNombreAsync(string nombre, int restauranteId, int? idExcluido = null);

    }
}
