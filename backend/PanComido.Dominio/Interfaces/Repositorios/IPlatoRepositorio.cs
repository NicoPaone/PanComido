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
        Task CrearAsync(Plato plato);
        Task ActualizarAsync(Plato plato);
    }
}
