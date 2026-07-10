using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso.Resultados;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class AnotarseEnFilaMesaCasoDeUso
    {
        private readonly ITurnoFilaRepositorio _turnoFilaRepositorio;
        private readonly ObtenerEstadoFilaMesaCasoDeUso _obtenerEstadoFilaMesaCasoDeUso;
        private readonly IMesaRepositorio _mesaRepositorio;
        private readonly IFilaVirtualNotificador _notificador;
        private readonly IMesaNotificador _mesaNotificador;

        public AnotarseEnFilaMesaCasoDeUso(
            ITurnoFilaRepositorio turnoFilaRepositorio,
            ObtenerEstadoFilaMesaCasoDeUso obtenerEstadoFilaMesaCasoDeUso,
            IMesaRepositorio mesaRepositorio,
            IFilaVirtualNotificador notificador,
            IMesaNotificador mesaNotificador)
        {
            _turnoFilaRepositorio = turnoFilaRepositorio;
            _obtenerEstadoFilaMesaCasoDeUso = obtenerEstadoFilaMesaCasoDeUso;
            _mesaRepositorio = mesaRepositorio;
            _notificador = notificador;
            _mesaNotificador = mesaNotificador;
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

            var mesasLibres = await _mesaRepositorio.ObtenerDisponiblesAsync(restauranteId);
            var mesaIdeal = mesasLibres
                .Where(m => m.CantPersonasMax >= cantComensales)
                .OrderBy(m => m.CantPersonasMax) // Buscar la mesa más chica que se adapte
                .FirstOrDefault();

            if (mesaIdeal != null)
            {
                nuevoTurno.Estado = EstadoTurnoMesa.MesaAsignada;
                nuevoTurno.MesaAsignadaId = mesaIdeal.Id;
                nuevoTurno.FechaHoraAsignacion = DateTime.UtcNow;
                
                await _turnoFilaRepositorio.ActualizarAsync(nuevoTurno);

                await _mesaRepositorio.ActualizarEstadoAsync(mesaIdeal.Id, EstadoMesa.Reservada);

                _ = _notificador.NotificarMesaListaAsync(nuevoTurno.Id, mesaIdeal.Id, 7);

                mesaIdeal.EstadoMesa = EstadoMesa.Reservada;
                await _mesaNotificador.NotificarMesaActualizadaAsync(mesaIdeal, restauranteId);
            }

            var estadoFila = await _obtenerEstadoFilaMesaCasoDeUso.EjecutarAsync(nuevoTurno.Id);

            return new TurnoMesaResult
            {
                TurnoId = nuevoTurno.Id,
                NumeroTurno = nuevoTurno.Numero,
                TurnosAdelante = nuevoTurno.Estado == EstadoTurnoMesa.MesaAsignada ? 0 : turnosAdelante,
                TiempoEstimadoMinutos = nuevoTurno.Estado == EstadoTurnoMesa.MesaAsignada ? 0 : estadoFila.TiempoEstimadoMinutos,
                MesaLista = estadoFila.MesaLista,
                MesaAsignadaId = estadoFila.MesaAsignadaId,
                MinutosRestantesParaOcupar = estadoFila.MinutosRestantesParaOcupar
            };
        }
    }
}
