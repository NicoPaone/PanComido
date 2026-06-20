using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Dominio.Interfaces.Servicios.MercadoPago;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.PagoCasoDeUso
{
    public class ConfirmarPagoMPCasoDeUso
    {
        private readonly IMercadoPagoServicio _mercadoPagoServicio;
        private readonly IPagoRepositorio _pagoRepositorio;
        private readonly IComandaRepositorio _comandaRepositorio;
        private readonly IComandaNotificador _comandaNotificador;
        private readonly ILogger<ConfirmarPagoMPCasoDeUso> _logger;

        public ConfirmarPagoMPCasoDeUso(
            IMercadoPagoServicio mercadoPagoServicio,
            IPagoRepositorio pagoRepositorio,
            IComandaRepositorio comandaRepositorio,
            IComandaNotificador comandaNotificador,
            ILogger<ConfirmarPagoMPCasoDeUso> logger)
        {
            _mercadoPagoServicio = mercadoPagoServicio;
            _pagoRepositorio = pagoRepositorio;
            _comandaRepositorio = comandaRepositorio;
            _comandaNotificador = comandaNotificador;
            _logger = logger;
        }

        public async Task<Pago?> EjecutarAsync(long paymentId)
        {
            ResultadoPagoMP resultado = await _mercadoPagoServicio.ConsultarPagoAsync(paymentId);
            Pago pagoAConfirmar = await _pagoRepositorio.ObtenerPagoPorExternalReferenceAsync(resultado.ExternalReference);

            if (pagoAConfirmar == null)
            {
                _logger.LogWarning("Pago no encontrado para ExternalReference: {ExternalReference}", resultado.ExternalReference);
                throw new KeyNotFoundException("El pago no fue encontrado");
            }

            Comanda comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(pagoAConfirmar.ComandaId);

            if (pagoAConfirmar.EstadoPago == EstadoPago.Confirmado) return null;


            if (resultado.Status != "approved")
            {
                _logger.LogWarning("Pago rechazado por MP. ExternalReference: {ExternalReference}, Status: {Status}", resultado.ExternalReference, resultado.Status);
                await _pagoRepositorio.RechazarPagoAsync(resultado.ExternalReference);
                await _comandaNotificador.NotificarPagoRechazadoAMesaAsync(comanda);
                return null;
            }

            await _pagoRepositorio.ConfirmarPagoAsync(resultado.ExternalReference);
    
            comanda.Estado = EstadoComanda.Finalizada;
            comanda.HoraFin = DateTime.Now;
            await _comandaRepositorio.ActualizarAsync(comanda);
            await _comandaNotificador.NotificarComandaActualizadaAMesaAsync(comanda);
            return pagoAConfirmar;
        }
    }
}
