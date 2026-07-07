using PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IReglaTiempoExtraRepositorio
    {
        Task<ReglaTiempoExtra> ObtenerPorIdAsync(int id);
        Task<List<ReglaTiempoExtra>> ObtenerPorRestauranteIdAsync(int restauranteId);
        Task<ReglaTiempoExtra> CrearAsync(ReglaTiempoExtra regla);
        Task<ReglaTiempoExtra> ActualizarAsync(ReglaTiempoExtra regla);
        Task EliminarAsync(int id);
    }
}
