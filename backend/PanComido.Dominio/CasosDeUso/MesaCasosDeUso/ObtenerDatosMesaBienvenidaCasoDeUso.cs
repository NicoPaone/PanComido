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
    public class ObtenerDatosMesaBienvenidaCasoDeUso
    {
        private readonly IRestauranteRepositorio _restauranteRepositorio;
        private readonly IMesaRepositorio _mesaRepositorio;

        private readonly ILogger<ObtenerDatosMesaBienvenidaCasoDeUso> _logger;

        public ObtenerDatosMesaBienvenidaCasoDeUso(IRestauranteRepositorio restauranteRepositorio, IMesaRepositorio mesaRepositorio, ILogger<ObtenerDatosMesaBienvenidaCasoDeUso> logger)
        {
            _restauranteRepositorio = restauranteRepositorio;
            _mesaRepositorio = mesaRepositorio;
            _logger = logger;
        }

        public async Task<BienvenidaMesaDatos> EjecutarAsync(int mesaId, int restauranteId)
        {
            await ValidarExistenciaRestauranteAsync(restauranteId);

            MesaConPosiciones mesa = await ObtenerYValidarMesaAsync(mesaId, restauranteId);

            return new BienvenidaMesaDatos
            {
                Mesa = mesa,
                RestauranteId = restauranteId
            };
        }

        private async Task ValidarExistenciaRestauranteAsync(int restauranteId)
        {
            Restaurante? restauranteDatos = await _restauranteRepositorio.ObtenerDatosDelLocalAsync(restauranteId);

            if (restauranteDatos == null)
            {
                _logger.LogWarning("Intento de escaneo de QR para un restaurante inexistente. RestauranteId: {RestauranteId}", restauranteId);
                throw new KeyNotFoundException("Restaurante no encontrado");
            }
        }

        private async Task<MesaConPosiciones> ObtenerYValidarMesaAsync(int mesaId, int restauranteId)
        {
            MesaConPosiciones mesa = await _mesaRepositorio.ObtenerPorIdAsync(mesaId, restauranteId);

            if (mesa == null)
            {
                _logger.LogWarning("Intento de escaneo de QR para una mesa inexistente. RestauranteId: {RestauranteId}, MesaId: {MesaId}", restauranteId, mesaId);
                throw new KeyNotFoundException("Mesa no encontrada");
            }

            if (mesa.EstadoMesa == EstadoMesa.Reservada || mesa.EstadoMesa == EstadoMesa.Deshabilitada)
            {
                _logger.LogWarning("Intento de escaneo de QR en una mesa inhabilitada para sentarse. RestauranteId: {RestauranteId}, MesaId: {MesaId}, EstadoActual: {EstadoMesa}", restauranteId, mesaId, mesa.EstadoMesa);
                throw new InvalidOperationException($"Esta mesa se encuentra {mesa.EstadoMesa.ToString()}");
            }

            if (mesa.EstadoMesa == EstadoMesa.Ocupada)
            {
                _logger.LogWarning("Intento de escaneo del QR principal en una mesa que ya está ocupada. RestauranteId: {RestauranteId}, MesaId: {MesaId}", restauranteId, mesaId);
                throw new InvalidOperationException("La mesa ya está ocupada, puede pedirle al comensal anfitrion el QR de invitacion");
            }

            return mesa;
        }

    }
}
