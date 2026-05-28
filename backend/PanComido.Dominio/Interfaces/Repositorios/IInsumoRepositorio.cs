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
        
        // Pendientes

        // Task<Insumo> ObtenerInsumoPorIdAsync(int restauranteId, int idInsumo);
        // Task<List<Insumo>> ObtenerInsumosPorCategoriaAsync(int restauranteId, string categoria);
        // Task<List<Insumo>> ObtenerInsumosPorBusquedaAsync(int restauranteId, string busqueda);
        //Task AgregarInsumoAsync(Insumo insumo);
        // Task ActualizarInsumoAsync(Insumo insumo);
        // Task EliminarInsumoAsync(int id);
    }
}
