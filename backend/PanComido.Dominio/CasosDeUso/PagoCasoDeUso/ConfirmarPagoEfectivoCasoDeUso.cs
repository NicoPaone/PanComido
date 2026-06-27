using Microsoft.Extensions.Logging;
using PanComido.Dominio.CasosDeUso.ComandaCasosDeUso;
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
    public class ConfirmarPagoEfectivoCasoDeUso
    {
        private readonly IPagoRepositorio _pagoRepositorio;
        private readonly IComandaRepositorio _comandaRepositorio;
        private readonly ILlamadoRepositorio _llamadoRepositorio;
        private readonly ICalcularTotalComandaServicio _calcularTotalComandaServicio;
        private readonly IComandaNotificador _comandaNotificador;
        private readonly ILogger<ConfirmarPagoEfectivoCasoDeUso> _logger;

        public ConfirmarPagoEfectivoCasoDeUso(IPagoRepositorio pagoRepositorio, IComandaRepositorio comandaRepositorio, ILlamadoRepositorio llamadoRepositorio, ICalcularTotalComandaServicio calcularTotalComandaServicio,
            IComandaNotificador comandaNotificador, ILogger<ConfirmarPagoEfectivoCasoDeUso> logger)
        {
            _pagoRepositorio = pagoRepositorio;
            _comandaRepositorio = comandaRepositorio;
            _llamadoRepositorio = llamadoRepositorio;
            _calcularTotalComandaServicio = calcularTotalComandaServicio;
            _comandaNotificador = comandaNotificador;
            _logger = logger;
        }

        public async Task<Pago> EjecutarAsync(int comandaId, int restauranteId)
        {
            var comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);
            if (comanda == null || comanda.RestauranteId != restauranteId) throw new KeyNotFoundException("Comanda no encontrada");
            if (comanda.Estado != EstadoComanda.EnEspera)
            {
                _logger.LogWarning("Intento de confirmar pago efectivo en estado inválido. ComandaId: {ComandaId}, Estado: {Estado}", comandaId, comanda.Estado);
                throw new ArgumentException("La comanda no está esperando pago.");
            }

            decimal totalComanda = _calcularTotalComandaServicio.CalcularTotal(comanda);

            Pago pagoExistente = await _pagoRepositorio.ObtenerPagoPorComandaIdAsync(comandaId);
            if (pagoExistente != null && pagoExistente.EstadoPago == EstadoPago.Confirmado)
            {
                _logger.LogWarning("Intento de confirmar pago ya confirmado (idempotencia). ComandaId: {ComandaId}", comandaId);
                throw new InvalidOperationException("El pago ya fue confirmado");
            }

            Pago pago = new Pago
            {
                MetodoDePago = MetodoPago.Efectivo,
                Total = totalComanda,
                ComandaId = comandaId,
                EstadoPago = EstadoPago.Confirmado
            };

            Pago pagoCreado = await _pagoRepositorio.CrearPagoAsync(pago);

            comanda.Estado = EstadoComanda.Finalizada;
            comanda.HoraFin = DateTime.Now;
            await _comandaRepositorio.ActualizarAsync(comanda);

            List<int> mozosId = new List<int>();
            mozosId.Add(comanda.MozoId.Value);

            await _comandaNotificador.NotificarEstadoModificadoAsync(comanda, mozosId);

            await _llamadoRepositorio.ResolverLlamadoPorMesaYCategoriaAsync(comanda.MesaId, (int)CategoriaLlamado.Pago);

            _logger.LogInformation("Pago efectivo confirmado. ComandaId: {ComandaId}, Total: {Total}", comandaId, totalComanda);
            return pagoCreado;
        }
    }
}
