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
    public class SolicitarPagoEfectivoCasoDeUso
    {
        private readonly IComandaRepositorio _comandaRepositorio;
        private readonly ILlamadoRepositorio _llamadoRepositorio;
        private readonly ILlamadoNotificador _llamadoNotificador;
        private readonly ILogger<SolicitarPagoEfectivoCasoDeUso> _logger;


        public SolicitarPagoEfectivoCasoDeUso(IComandaRepositorio comandaRepositorio, ILlamadoRepositorio llamadoRepositorio,
            ILlamadoNotificador llamadoNotificador, ILogger<SolicitarPagoEfectivoCasoDeUso> logger)
        {
            _comandaRepositorio = comandaRepositorio;
            _llamadoRepositorio = llamadoRepositorio;
            _llamadoNotificador = llamadoNotificador;
            _logger = logger;
        }

        public async Task<Llamado> EjecutarAsync(int comandaId, int restauranteId)
        {
            Comanda comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);
            if (comanda == null || comanda.RestauranteId != restauranteId)
            {
                _logger.LogWarning("Comanda no encontrada para solicitud de pago efectivo. ComandaId: {ComandaId}, RestauranteId: {RestauranteId}", comandaId, restauranteId);
                throw new KeyNotFoundException("Comanda no encontrada para el restaurante especificado.");
            }

            if (comanda.Estado != EstadoComanda.EnEspera)
            {
                _logger.LogWarning("Intento de solicitar pago efectivo en estado inválido. ComandaId: {ComandaId}, Estado: {Estado}", comandaId, comanda.Estado);
                throw new ArgumentException("La comanda no está esperando pago.");
            }

            Llamado llamado = new Llamado
            {
                MozoId = comanda.MozoId,
                MesaId = comanda.MesaId,
                CategoriaLlamadoId = 7,
                Descripcion = "El comensal ha solicitado el pago en efectivo.",
                Resuelto = false
            };

            Llamado llamadoCreado = await _llamadoRepositorio.crearLlamadoAsync(llamado);
            await _llamadoNotificador.NotificarLlamadoAsync(llamadoCreado);
            _logger.LogInformation("Pago efectivo solicitado. ComandaId: {ComandaId}, MesaId: {MesaId}, MozoId: {MozoId}", comandaId, comanda.MesaId, comanda.MozoId);
            return llamadoCreado;
        }
    }
}
