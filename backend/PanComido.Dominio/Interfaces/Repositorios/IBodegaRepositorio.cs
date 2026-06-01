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
    }
}
