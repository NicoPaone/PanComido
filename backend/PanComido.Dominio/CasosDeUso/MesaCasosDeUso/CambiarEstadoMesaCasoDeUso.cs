using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class CambiarEstadoMesaCasoDeUso
    {
        private readonly IMesaRepositorio _mesaRepositorio;
        private readonly ILlamadoNotificador _llamadoNotificador;
        private readonly ILlamadoRepositorio _llamadoRepositorio;
        private readonly IMesaNotificador _mesaNotificador;
        private readonly ITurnoFilaRepositorio _turnoFilaRepositorio;
        private readonly IFilaVirtualNotificador _filaVirtualNotificador;
        private readonly IComandaRepositorio _comandaRepositorio;

        public CambiarEstadoMesaCasoDeUso(
            IMesaRepositorio mesaRepositorio, 
            ILlamadoNotificador llamadoNotificador, 
            ILlamadoRepositorio llamadoRepositorio, 
            IMesaNotificador mesaNotificador,
            ITurnoFilaRepositorio turnoFilaRepositorio,
            IFilaVirtualNotificador filaVirtualNotificador,
            IComandaRepositorio comandarepositorio)
        {
            _mesaRepositorio = mesaRepositorio;
            _llamadoNotificador = llamadoNotificador;
            _llamadoRepositorio = llamadoRepositorio;
            _mesaNotificador = mesaNotificador;
            _turnoFilaRepositorio = turnoFilaRepositorio;
            _filaVirtualNotificador = filaVirtualNotificador;
            _comandaRepositorio = comandarepositorio;
        }

        public async Task<MesaConPosiciones> EjecutarAsync(int restauranteId, int mesaId, EstadoMesa nuevoEstado)
        {
            MesaConPosiciones mesa = await _mesaRepositorio.ObtenerPorIdAsync(mesaId, restauranteId);

            if (mesa == null)
                throw new ArgumentException("La mesa no existe o no pertenece al restaurante.");

            if(nuevoEstado == EstadoMesa.Disponible)
            {
                var comandaActiva = await _comandaRepositorio.ObtenerComandaPorIdMesaAsync(mesaId);
                if (comandaActiva != null)
                    throw new InvalidOperationException("No se puede cambiar el estado de la mesa mientras tenga una comanda activa.");
            }

            mesa.EstadoMesa = nuevoEstado;
            await _mesaRepositorio.ActualizarEstadoAsync(mesaId, nuevoEstado);
            if (mesa.EstadoMesa == EstadoMesa.Disponible)
            {
                List<Llamado> llamadosResueltos = await _llamadoRepositorio.ResolverTodosLosPendientesPorMesaAsync(mesaId);
                if (llamadosResueltos.Any())
                    await _llamadoNotificador.NotificarLlamadosResueltosAsync(mesaId, llamadosResueltos);

                int filaVirtualId = await _turnoFilaRepositorio.ObtenerFilaVirtualIdAsync(restauranteId);
                if (filaVirtualId > 0)
                {
                    var proximoTurno = await _turnoFilaRepositorio.ObtenerProximoTurnoEnEsperaAsync(filaVirtualId, mesa.CantPersonasMax);
                    if (proximoTurno != null)
                    {
                        proximoTurno.Estado = EstadoTurnoMesa.MesaAsignada;
                        proximoTurno.MesaAsignadaId = mesa.Id;
                        proximoTurno.FechaHoraAsignacion = DateTime.UtcNow;
                        
                        await _turnoFilaRepositorio.ActualizarAsync(proximoTurno);
                        await _filaVirtualNotificador.NotificarMesaListaAsync(proximoTurno.Id, mesa.Id, 7);
                        
                        mesa.EstadoMesa = EstadoMesa.Reservada;
                        await _mesaRepositorio.ActualizarEstadoAsync(mesaId, EstadoMesa.Reservada);
                    }
                }
            }
            await _mesaNotificador.NotificarMesaActualizadaAsync(mesa, restauranteId);
            return mesa;
        }
    }
}
