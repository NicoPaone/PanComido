using PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IInsumoRepositorio
    {
        Task<List<Insumo>> ObtenerInsumosAsync(int restauranteId);
        Task<List<Insumo>> ObtenerInsumosDelProveedorAsync(int proveedorId, int restauranteId);
        Task <Insumo> CrearAsync(Insumo insumo);
        Task<bool> ExisteInsumoConNombreAsync(int restauranteId, string nombre);
        Task<List<Insumo>> ObtenerInsumosConLotesAsync(int restauranteId);
        Task<List<Insumo>> ObtenerInsumosProximosAVencerAsync(int restauranteId);
        Task<Insumo> ObtenerPorIdAsync(int insumoId, int restauranteId);
        Task<Insumo> ActualizarAsync(Insumo insumo);
        Task<Insumo> EliminarAsync(int insumoId, int restauranteId);
        Task<bool> ExistenInsumosActivosAsync(List<int> insumoIds, int restauranteId);
    }
}