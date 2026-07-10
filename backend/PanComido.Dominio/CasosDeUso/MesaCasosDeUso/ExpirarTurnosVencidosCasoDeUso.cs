using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
    public interface IExpirarTurnosVencidosCasoDeUso
    {
        Task EjecutarAsync();
    }

    public class ExpirarTurnosVencidosCasoDeUso : IExpirarTurnosVencidosCasoDeUso
    {
        private readonly ITurnoFilaRepositorio _turnoFilaRepositorio;
        private readonly IMesaRepositorio _mesaRepositorio;
        private readonly IFilaVirtualNotificador _filaVirtualNotificador;
        private readonly IMesaNotificador _mesaNotificador;

        public ExpirarTurnosVencidosCasoDeUso(
            ITurnoFilaRepositorio turnoFilaRepositorio,
            IMesaRepositorio mesaRepositorio,
            IFilaVirtualNotificador filaVirtualNotificador,
            IMesaNotificador mesaNotificador)
        {
            _turnoFilaRepositorio = turnoFilaRepositorio;
            _mesaRepositorio = mesaRepositorio;
            _filaVirtualNotificador = filaVirtualNotificador;
            _mesaNotificador = mesaNotificador;
        }

        public async Task EjecutarAsync()
        {
            var fechaLimite = DateTime.UtcNow.AddMinutes(-7);
            var turnosExpirados = await _turnoFilaRepositorio.ObtenerTurnosAsignadosExpiradosAsync(fechaLimite);

            Console.WriteLine($"[ExpirarTurnos] Evaluando {turnosExpirados.Count} turnos expirados antes de {fechaLimite:HH:mm:ss}");

            foreach (var turno in turnosExpirados)
            {
                Console.WriteLine($"[ExpirarTurnos] Cancelando turno {turno.Id}");
                // 1. Cambiamos el estado a Cancelado
                turno.Estado = EstadoTurnoMesa.Cancelado;
                await _turnoFilaRepositorio.ActualizarAsync(turno);

                // 2. Notificamos al usuario expirado
                string msjExpulsion = $"Ingresó a la fila el {turno.FechaHoraIngreso.ToLocalTime():dd/MM} a las {turno.FechaHoraIngreso.ToLocalTime():HH:mm}. Su tiempo de espera de 7 minutos caducó sin presentarse. Por favor, acérquese y comuníquese con la recepcionista para solucionar su turno.";
                await _filaVirtualNotificador.NotificarTurnoExpiradoAsync(turno.Id, msjExpulsion);

                // 3. Reasignar la mesa al próximo en la fila si es que lo hay
                if (turno.MesaAsignadaId.HasValue)
                {
                    int mesaId = turno.MesaAsignadaId.Value;
                    
                    // Necesitamos saber la capacidad de la mesa para buscar el próximo turno compatible
                    // En PanComido la mesa se busca cruzando Id y RestauranteId, pero como no tenemos RestauranteId aca facil,
                    // asumo que el repositorio tiene un ObtenerTodas o podemos usar los Ids de Restaurante indirectos.
                    // Pero para optimizar buscaremos la fila virtual y a partir de ahí.
                    var filaVirtual = await _turnoFilaRepositorio.ObtenerFilaVirtualPorIdAsync(turno.FilaVirtualId);
                    
                    var mesa = await _mesaRepositorio.ObtenerPorIdAsync(mesaId, filaVirtual.RestauranteId);
                    
                    if (mesa != null && mesa.EstadoMesa == EstadoMesa.Reservada)
                    {
                        var proximoTurno = await _turnoFilaRepositorio.ObtenerProximoTurnoEnEsperaAsync(turno.FilaVirtualId, mesa.CantPersonasMax);
                        
                        if (proximoTurno != null)
                        {
                            // Le asignamos la mesa al nuevo turno
                            proximoTurno.Estado = EstadoTurnoMesa.MesaAsignada;
                            proximoTurno.MesaAsignadaId = mesa.Id;
                            proximoTurno.FechaHoraAsignacion = DateTime.UtcNow;
                            
                            await _turnoFilaRepositorio.ActualizarAsync(proximoTurno);
                            await _filaVirtualNotificador.NotificarMesaListaAsync(proximoTurno.Id, mesa.Id, 7);
                        }
                        else
                        {
                            // Si no hay nadie en espera compatible, liberamos la mesa
                            mesa.EstadoMesa = EstadoMesa.Disponible;
                            await _mesaRepositorio.ActualizarEstadoAsync(mesaId, EstadoMesa.Disponible);
                            await _mesaNotificador.NotificarMesaActualizadaAsync(mesa, filaVirtual.RestauranteId);
                        }
                    }
                }
            }
        }
    }
}
