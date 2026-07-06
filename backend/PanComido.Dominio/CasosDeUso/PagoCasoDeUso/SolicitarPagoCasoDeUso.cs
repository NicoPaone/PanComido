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

namespace PanComido.Dominio.CasosDeUso.PagoCasoDeUso
{
    public class SolicitarPagoCasoDeUso
    {
        private readonly IComandaRepositorio _comandaRepositorio;
        private readonly ICrearLlamadoServicio _crearLlamadoServicio;
        private readonly IVerificarMetodoPagoHabilitadoServicio _verificarMetodoPagoHabilitadoServicio;
        private readonly ILogger<SolicitarPagoCasoDeUso> _logger;

        public SolicitarPagoCasoDeUso(IComandaRepositorio comandaRepositorio, ICrearLlamadoServicio crearLlamadoServicio, IVerificarMetodoPagoHabilitadoServicio verificarMetodoPagoHabilitadoServicio, ILogger<SolicitarPagoCasoDeUso> logger)
        {
            _comandaRepositorio = comandaRepositorio;
            _crearLlamadoServicio = crearLlamadoServicio;
            _verificarMetodoPagoHabilitadoServicio = verificarMetodoPagoHabilitadoServicio;
            _logger = logger;
        }

        public async Task<Llamado> EjecutarAsync(int comandaId, int restauranteId, MetodoPago metodoPago)
        {
            var comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);
            if (comanda == null || comanda.RestauranteId != restauranteId)
            {
                _logger.LogWarning("Comanda no encontrada para solicitud de pago. ComandaId: {ComandaId}, RestauranteId: {RestauranteId}", comandaId, restauranteId);
                throw new KeyNotFoundException("Comanda no encontrada para el restaurante especificado.");
            }
            //if (comanda.Estado != EstadoComanda.EnEspera)
            //{
            //    _logger.LogWarning("Intento de solicitar pago en estado inválido. ComandaId: {ComandaId}, Estado: {Estado}", comandaId, comanda.Estado);
            //    throw new ArgumentException("La comanda no está esperando pago.");
            //}

            bool metodoHabilitado = await _verificarMetodoPagoHabilitadoServicio.EstaHabilitadoAsync(restauranteId, metodoPago);
            if (!metodoHabilitado)
            {
                _logger.LogWarning("Intento de solicitar pago con un método no habilitado. ComandaId: {ComandaId}, Metodo: {Metodo}", comandaId, metodoPago);
                throw new ArgumentException("El método de pago solicitado no está habilitado para este restaurante.");
            }

            string mensajePago = metodoPago switch
            {
                MetodoPago.Efectivo => "El comensal ha solicitado pagar en efectivo.",
                MetodoPago.Tarjeta => "El comensal ha solicitado pagar con tarjeta.",
                MetodoPago.Transferencia => "El comensal ha solicitado pagar mediante transferencia.",
                _ => throw new ArgumentOutOfRangeException(nameof(metodoPago), "Método de pago no soportado.")
            };

            Llamado llamadoGuardado = await _crearLlamadoServicio.CrearYNotificarAsync(comanda.MozoId, comanda.MesaId, comanda.NumeroDeMesa, CategoriaLlamado.Pago, mensajePago);

            _logger.LogInformation("Pago solicitado. ComandaId: {ComandaId}, MesaId: {MesaId}, MozoId: {MozoId}", comandaId, comanda.MesaId, comanda.MozoId);
            return llamadoGuardado;
        }
    }
}
