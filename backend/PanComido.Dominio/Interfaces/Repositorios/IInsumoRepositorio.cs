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
        Task CrearAsync(Insumo insumo);
        Task<List<Insumo>> ObtenerInsumosConLotesAsync(int restauranteId);
    }
}