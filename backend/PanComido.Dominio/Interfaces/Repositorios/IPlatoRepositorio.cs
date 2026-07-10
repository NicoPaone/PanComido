using PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IPlatoRepositorio
    {
        Task<Plato> ObtenerPorIdAsync(int platoId, int restauranteId);
        Task<bool> ExistePlatoConNombreAsync(int restauranteId, string nombre);
        Task CrearAsync(Plato plato);
        Task ActualizarAsync(Plato plato);
        Task EliminarAsync(int platoId, int restauranteId);
        Task<bool> ExisteInsumoEnPlatosActivosAsync(int insumoId);
    }
}
