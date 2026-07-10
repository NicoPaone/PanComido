using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class CancelarTurnoFilaCasoDeUso
    {
        private readonly ITurnoFilaRepositorio _turnoFilaRepositorio;

        public CancelarTurnoFilaCasoDeUso(ITurnoFilaRepositorio turnoFilaRepositorio)
        {
            _turnoFilaRepositorio = turnoFilaRepositorio;
        }

        public async Task EjecutarAsync(int turnoId)
        {
            var turno = await _turnoFilaRepositorio.ObtenerPorIdAsync(turnoId);
            if (turno == null) throw new ArgumentException("Turno de fila no encontrado");

            if (turno.Estado == EstadoTurnoMesa.Completado || turno.Estado == EstadoTurnoMesa.Cancelado)
            {
                throw new InvalidOperationException("El turno ya está completado o cancelado.");
            }

            turno.Estado = EstadoTurnoMesa.Cancelado;
            
            // Actualizar a través del repositorio
            await _turnoFilaRepositorio.ActualizarAsync(turno);
        }
    }
}
