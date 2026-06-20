using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
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

        private readonly ILogger<OcuparMesaCasoDeUso> _logger;

        public OcuparMesaCasoDeUso(IMesaRepositorio mesaRepositorio, IComandaRepositorio comandaRepositorio, ILogger<OcuparMesaCasoDeUso> logger)
        {
            _mesaRepositorio = mesaRepositorio;
            _comandaRepositorio = comandaRepositorio;
            _logger = logger;
        }

        public async Task<MesaConPosiciones> EjecutarAsync(int restauranteId, int mesaId, int cantComensales)
        {
            _logger.LogInformation("Iniciando ocupación de la mesa {MesaId} en el restaurante {RestauranteId} para {CantComensales} comensales.", mesaId, restauranteId, cantComensales);

            MesaConPosiciones mesa = await _mesaRepositorio.ObtenerPorIdAsync(mesaId, restauranteId);

            if (mesa == null)
            {
                _logger.LogWarning("Rechazo al ocupar mesa: La mesa {MesaId} no existe o no pertenece al restaurante {RestauranteId}.", mesaId, restauranteId);
                throw new ArgumentException("La mesa no existe o no pertenece al restaurante.");
            }

            if (mesa.EstadoMesa != EstadoMesa.Disponible)
            {
                _logger.LogWarning("Rechazo al ocupar mesa: La mesa {MesaId} en el restaurante {RestauranteId} se encuentra en estado '{EstadoMesa}' en lugar de Disponible.", mesaId, restauranteId, mesa.EstadoMesa);
                throw new InvalidOperationException("La mesa no está disponible para ser ocupada.");
            }

            if (cantComensales > mesa.CantPersonasMax)
            {
                _logger.LogWarning("Rechazo al ocupar mesa: La cantidad de comensales solicitada ({CantComensales}) supera la capacidad máxima ({CapacidadMaxima}) de la mesa {MesaId}.", cantComensales, mesa.CantPersonasMax, mesaId);
                throw new InvalidOperationException("La cantidad de comensales excede la capacidad máxima de la mesa.");
            }

            mesa.EstadoMesa = EstadoMesa.Ocupada;
            await _mesaRepositorio.ActualizarEstadoAsync(mesaId,EstadoMesa.Ocupada);


            Comanda nuevaComanda = new Comanda
            {
                MesaId = mesa.Id,
                RestauranteId = restauranteId,
                Estado = EstadoComanda.Abierta,
                CantComensales = cantComensales,
                HoraInicio = DateTime.Now
            };

            int idComanda = await _comandaRepositorio.CrearAsync(nuevaComanda);

            mesa.idComanda = idComanda;

            _logger.LogInformation("Mesa {MesaId} ocupada exitosamente. Se creó la nueva comanda {ComandaId} para {CantComensales} comensales.", mesaId, idComanda, cantComensales);
            return mesa;

        }
    }
}
