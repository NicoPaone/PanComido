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

    public class ConfirmarPagoCasoDeUso
    {
        private readonly IPagoRepositorio _pagoRepositorio;
        private readonly IComandaRepositorio _comandaRepositorio;
        private readonly ILlamadoRepositorio _llamadoRepositorio;
        private readonly ICalcularTotalComandaServicio _calcularTotalComandaServicio;
        private readonly IComandaNotificador _comandaNotificador;
        private readonly ILlamadoNotificador _llamadoNotificador;
        private readonly IRegistrarPagoServicio _registrarPagoServicio;
        private readonly IVerificarMetodoPagoHabilitadoServicio _verificarMetodoPagoHabilitadoServicio;
        private readonly ILogger<ConfirmarPagoCasoDeUso> _logger;

        public ConfirmarPagoCasoDeUso(IPagoRepositorio pagoRepositorio, IComandaRepositorio comandaRepositorio, ILlamadoRepositorio llamadoRepositorio, ICalcularTotalComandaServicio calcularTotalComandaServicio,
IComandaNotificador comandaNotificador, ILlamadoNotificador llamadoNotificador,
IRegistrarPagoServicio registrarPagoServicio, IVerificarMetodoPagoHabilitadoServicio verificarMetodoPagoHabilitadoServicio, ILogger<ConfirmarPagoCasoDeUso> logger)
        {
            _pagoRepositorio = pagoRepositorio;
            _comandaRepositorio = comandaRepositorio;
            _llamadoRepositorio = llamadoRepositorio;
            _calcularTotalComandaServicio = calcularTotalComandaServicio;
            _comandaNotificador = comandaNotificador;
            _llamadoNotificador = llamadoNotificador;
            _registrarPagoServicio = registrarPagoServicio;
            _verificarMetodoPagoHabilitadoServicio = verificarMetodoPagoHabilitadoServicio;
            _logger = logger;
        }

        public async Task<Pago> EjecutarAsync(int comandaId, int restauranteId, MetodoPago metodoPago)
        {
            if (metodoPago == MetodoPago.MercadoPago)
                throw new ArgumentException("El pago con Mercado Pago se confirma mediante webhook, no manualmente.");

            var comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);
            if (comanda == null || comanda.RestauranteId != restauranteId) throw new
        KeyNotFoundException("Comanda no encontrada");
            await VerificarEstadoComandaYPagoAsync(comanda);
            await VerificarMetodoHabilitadoAsync(restauranteId, metodoPago);

            decimal total = _calcularTotalComandaServicio.CalcularTotal(comanda);
            Pago pagoCreado = await _registrarPagoServicio.RegistrarAsync(comanda.Id, total, metodoPago, EstadoPago.Confirmado);
            await FinalizarComandaYNotificarAsync(comanda);

            _logger.LogInformation("Pago confirmado. ComandaId: {ComandaId}, Metodo: { Metodo}, Total: { Total}", comandaId, metodoPago, pagoCreado.Total);
            return pagoCreado;
        }

        private async Task VerificarEstadoComandaYPagoAsync(Comanda comanda)
        {
            //if (comanda.Estado != EstadoComanda.EnEspera)
            //{
            //    _logger.LogWarning("Intento de confirmar pago efectivo en estado inválido. ComandaId: {ComandaId}, Estado: {Estado}", comanda.Id, comanda.Estado);
            //    throw new ArgumentException("La comanda no está esperando pago.");
            //}

            Pago pagoExistente = await _pagoRepositorio.ObtenerPagoPorComandaIdAsync(comanda.Id);
            if (pagoExistente != null && pagoExistente.EstadoPago == EstadoPago.Confirmado)
            {
                _logger.LogWarning("Intento de confirmar pago ya confirmado (idempotencia). ComandaId: {ComandaId}", comanda.Id);
                throw new InvalidOperationException("El pago ya fue confirmado");
            }
        }

        private async Task VerificarMetodoHabilitadoAsync(int restauranteId, MetodoPago metodoPago)
        {
            bool metodoHabilitado = await _verificarMetodoPagoHabilitadoServicio.EstaHabilitadoAsync(restauranteId, metodoPago);
            if (!metodoHabilitado)
            {
                _logger.LogWarning("Intento de confirmar pago con un método no habilitado. RestauranteId: {RestauranteId}, Metodo: {Metodo}", restauranteId, metodoPago);
                throw new ArgumentException("El método de pago no está habilitado para este restaurante.");
            }
        }

        private async Task FinalizarComandaYNotificarAsync(Comanda comanda)
        {
            comanda.Estado = EstadoComanda.Finalizada;
            comanda.HoraFin = DateTime.Now;
            await _comandaRepositorio.ActualizarAsync(comanda);

            await _comandaNotificador.NotificarEstadoModificadoAsync(comanda, new List<int> { comanda.MozoId.Value });

            Llamado? llamado = await _llamadoRepositorio.ResolverLlamadoPorMesaYCategoriaAsync(comanda.MesaId, (int)CategoriaLlamado.Pago);

            if (llamado != null)
                await _llamadoNotificador.NotificarLlamadosResueltosAsync(comanda.MesaId, new List<Llamado> { llamado });
        }

    }


}
