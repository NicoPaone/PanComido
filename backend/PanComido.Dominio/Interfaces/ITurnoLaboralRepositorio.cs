using PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces
{
    public interface ITurnoLaboralRepositorio
    {
        Task<List<TurnoLaboral>> ObtenerTurnosLaboralesAsync(int restauranteId);
        Task<List<TurnoLaboral>> ActualizarTurnosLaboralesAsync(int restauranteId, List<TurnoLaboral> turnos);
    }
}
