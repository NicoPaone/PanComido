using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class OcuparMesaCasoDeUso
    {
        private readonly IMesaRepositorio _mesaRepositorio;
        private readonly IComandaRepositorio _comandaRepositorio;
        private readonly ITurnoFilaRepositorio _turnoFilaRepositorio;
        private readonly IMesaNotificador _mesaNotificador;
        private readonly ILogger<OcuparMesaCasoDeUso> _logger;

        public OcuparMesaCasoDeUso(
            IMesaRepositorio mesaRepositorio, 
            IComandaRepositorio comandaRepositorio, 
            ITurnoFilaRepositorio turnoFilaRepositorio,
            IMesaNotificador mesaNotificador, 
            ILogger<OcuparMesaCasoDeUso> logger)
        {
            _mesaRepositorio = mesaRepositorio;
            _comandaRepositorio = comandaRepositorio;
            _turnoFilaRepositorio = turnoFilaRepositorio;
            _mesaNotificador = mesaNotificador;
            _logger = logger;
        }

        public async Task<MesaConPosiciones> EjecutarAsync(int restauranteId, int mesaId, int cantComensales, int? turnoId = null)
        {
            _logger.LogInformation("Iniciando ocupación de la mesa {MesaId} en el restaurante {RestauranteId} para {CantComensales} comensales. TurnoId: {TurnoId}", mesaId, restauranteId, cantComensales, turnoId);

            MesaConPosiciones mesa = await ObtenerYValidarMesaAsync(restauranteId, mesaId, cantComensales);

            mesa.EstadoMesa = EstadoMesa.Ocupada;
            await _mesaRepositorio.ActualizarEstadoAsync(mesaId, EstadoMesa.Ocupada);

            if (turnoId.HasValue)
            {
                // FLUJO FILA VIRTUAL
                var turno = await _turnoFilaRepositorio.ObtenerPorIdAsync(turnoId.Value);
                if (turno != null)
                {
                    turno.Estado = EstadoTurnoMesa.Completado;
                    await _turnoFilaRepositorio.ActualizarAsync(turno);

                    if (turno.ComandaPreArmadaId.HasValue)
                    {
                        mesa.idComanda = turno.ComandaPreArmadaId.Value;
                        
                        var comandaPreArmada = await _comandaRepositorio.ObtenerComandaPorIdAsync(mesa.idComanda.Value);
                        if (comandaPreArmada != null)
                        {
                            comandaPreArmada.MesaId = mesaId;
                            comandaPreArmada.Estado = EstadoComanda.EnEspera; // Borrador, no dispara a cocina
                            await _comandaRepositorio.ActualizarAsync(comandaPreArmada);
                        }
                        _logger.LogInformation("Mesa {MesaId} ocupada por cliente de Fila Virtual. Se asoció la comanda precargada {ComandaId} en estado EnEspera.", mesaId, mesa.idComanda);
                    }
                    else
                    {
                        // Fallback por si el turno no tenía comanda precargada por algún error
                        mesa.idComanda = await GenerarComandaParaMesaOcupadaAsync(mesaId, restauranteId, cantComensales);
                        _logger.LogInformation("Mesa {MesaId} ocupada (Fila Virtual sin comanda). Se creó nueva comanda {ComandaId}.", mesaId, mesa.idComanda);
                    }
                }
            }
            else
            {
                // FLUJO WALK-IN CLÁSICO
                mesa.idComanda = await GenerarComandaParaMesaOcupadaAsync(mesaId, restauranteId, cantComensales);
                _logger.LogInformation("Mesa {MesaId} ocupada exitosamente (Walk-in). Se creó la nueva comanda {ComandaId} en estado Abierta.", mesaId, mesa.idComanda);
            }
            
            await _mesaNotificador.NotificarMesaActualizadaAsync(mesa, restauranteId);
            
            return mesa;
        }

        private async Task<MesaConPosiciones> ObtenerYValidarMesaAsync(int restauranteId, int mesaId, int cantComensales)
        {
            MesaConPosiciones mesa = await _mesaRepositorio.ObtenerPorIdAsync(mesaId, restauranteId);

            if (mesa == null)
            {
                _logger.LogWarning("Rechazo al ocupar mesa: La mesa {MesaId} no existe o no pertenece al restaurante {RestauranteId}.", mesaId, restauranteId);
                throw new ArgumentException("La mesa no existe o no pertenece al restaurante.");
            }

            if (mesa.EstadoMesa != EstadoMesa.Disponible && mesa.EstadoMesa != EstadoMesa.Reservada)
            {
                _logger.LogWarning("Rechazo al ocupar mesa: La mesa {MesaId} en el restaurante {RestauranteId} se encuentra en estado '{EstadoMesa}' en lugar de Disponible o Reservada.", mesaId, restauranteId, mesa.EstadoMesa);
                throw new InvalidOperationException("La mesa no está disponible para ser ocupada.");
            }

            if (cantComensales > mesa.CantPersonasMax)
            {
                _logger.LogWarning("Rechazo al ocupar mesa: La cantidad de comensales solicitada ({CantComensales}) supera la capacidad máxima ({CapacidadMaxima}) de la mesa {MesaId}.", cantComensales, mesa.CantPersonasMax, mesaId);
                throw new InvalidOperationException("La cantidad de comensales excede la capacidad máxima de la mesa.");
            }

            return mesa;
        }

        private async Task<int> GenerarComandaParaMesaOcupadaAsync(int mesaId, int restauranteId, int cantComensales)
        {
            Comanda nuevaComanda = new Comanda
            {
                MesaId = mesaId,
                RestauranteId = restauranteId,
                Estado = EstadoComanda.Abierta,
                CantComensales = cantComensales,
                HoraInicio = DateTime.Now
            };

            return await _comandaRepositorio.CrearAsync(nuevaComanda);
        }

    }
}
