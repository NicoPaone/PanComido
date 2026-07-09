using PanComido.Dominio.Entidades;
using System;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface ITurnoFilaRepositorio
    {
        Task CrearAsync(TurnoFila turno);
        Task<TurnoFila> ObtenerPorIdAsync(int id);
        Task<int> ObtenerProximoNumeroTurnoAsync(int filaVirtualId);
        Task<int> ContarTurnosEnEsperaPreviosAsync(int filaVirtualId, DateTime fechaHoraIngreso);
        Task<int> ObtenerFilaVirtualIdAsync(int restauranteId);
        Task ActualizarAsync(TurnoFila turno);
        Task<FilaVirtual> ObtenerFilaVirtualPorIdAsync(int filaVirtualId);
        Task<System.Collections.Generic.List<TurnoFila>> ObtenerTurnosAsignadosExpiradosAsync(DateTime fechaLimite);
        Task<TurnoFila?> ObtenerProximoTurnoEnEsperaAsync(int filaVirtualId, int capacidadMesa);
    }
}
