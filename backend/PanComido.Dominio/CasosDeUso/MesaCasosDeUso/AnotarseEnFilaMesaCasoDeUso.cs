using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso.Resultados;
using System;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class AnotarseEnFilaMesaCasoDeUso
    {
        private readonly ITurnoFilaRepositorio _turnoFilaRepositorio;
        private readonly ObtenerEstadoFilaMesaCasoDeUso _obtenerEstadoFilaMesaCasoDeUso;

        public AnotarseEnFilaMesaCasoDeUso(
            ITurnoFilaRepositorio turnoFilaRepositorio,
            ObtenerEstadoFilaMesaCasoDeUso obtenerEstadoFilaMesaCasoDeUso)
        {
            _turnoFilaRepositorio = turnoFilaRepositorio;
            _obtenerEstadoFilaMesaCasoDeUso = obtenerEstadoFilaMesaCasoDeUso;
        }

        public async Task<TurnoMesaResult> EjecutarAsync(int restauranteId, int cantComensales)
        {
            var filaVirtualId = await _turnoFilaRepositorio.ObtenerFilaVirtualIdAsync(restauranteId);
            var numeroTurno = await _turnoFilaRepositorio.ObtenerProximoNumeroTurnoAsync(filaVirtualId);
            
            var nuevoTurno = new TurnoFila
            {
                FilaVirtualId = filaVirtualId,
                Numero = numeroTurno,
                CantidadComensales = cantComensales,
                FechaHoraIngreso = DateTime.UtcNow,
                Estado = EstadoTurnoMesa.EnEspera
            };

            await _turnoFilaRepositorio.CrearAsync(nuevoTurno);

            var turnosAdelante = await _turnoFilaRepositorio.ContarTurnosEnEsperaPreviosAsync(filaVirtualId, nuevoTurno.FechaHoraIngreso);
            var estadoFila = await _obtenerEstadoFilaMesaCasoDeUso.EjecutarAsync(nuevoTurno.Id);

            return new TurnoMesaResult
            {
                TurnoId = nuevoTurno.Id,
                NumeroTurno = nuevoTurno.Numero,
                TurnosAdelante = turnosAdelante,
                TiempoEstimadoMinutos = estadoFila.TiempoEstimadoMinutos
            };
        }
    }
}
