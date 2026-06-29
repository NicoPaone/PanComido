using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;


namespace PanComido.Dominio.CasosDeUso.PagoCasoDeUso
{
    public class ConfirmarPagoEfectivoCasoDeUso
    {
        private readonly IPagoRepositorio _pagoRepositorio;
        private readonly IComandaRepositorio _comandaRepositorio;
        private readonly ILlamadoRepositorio _llamadoRepositorio;
        private readonly ICalcularTotalComandaServicio _calcularTotalComandaServicio;
        private readonly IComandaNotificador _comandaNotificador;
        private readonly IRegistrarPagoServicio _registrarPagoServicio;
        private readonly ILogger<ConfirmarPagoEfectivoCasoDeUso> _logger;

        public ConfirmarPagoEfectivoCasoDeUso(IPagoRepositorio pagoRepositorio, IComandaRepositorio comandaRepositorio, ILlamadoRepositorio llamadoRepositorio, ICalcularTotalComandaServicio calcularTotalComandaServicio,
            IComandaNotificador comandaNotificador,
            IRegistrarPagoServicio registrarPagoServicio, ILogger<ConfirmarPagoEfectivoCasoDeUso> logger)
        {
            _pagoRepositorio = pagoRepositorio;
            _comandaRepositorio = comandaRepositorio;
            _llamadoRepositorio = llamadoRepositorio;
            _calcularTotalComandaServicio = calcularTotalComandaServicio;
            _comandaNotificador = comandaNotificador;
            _registrarPagoServicio = registrarPagoServicio;
            _logger = logger;
        }

        public async Task<Pago> EjecutarAsync(int comandaId, int restauranteId)
        {
            var comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);
            if (comanda == null || comanda.RestauranteId != restauranteId) throw new KeyNotFoundException("Comanda no encontrada");
            await VerificarEstadoComandaYPagoAsync(comanda);

            decimal total = _calcularTotalComandaServicio.CalcularTotal(comanda);
            Pago pagoCreado = await _registrarPagoServicio.RegistrarAsync(comanda.Id, total, MetodoPago.Efectivo,
            EstadoPago.Confirmado);
            await FinalizarComandaYNotificarAsync(comanda);

            _logger.LogInformation("Pago efectivo confirmado. ComandaId: {ComandaId}, Total: {Total}", comandaId, pagoCreado.Total);
            return pagoCreado;
        }

        private async Task VerificarEstadoComandaYPagoAsync(Comanda comanda)
        {
            if (comanda.Estado != EstadoComanda.EnEspera)
            {
                _logger.LogWarning("Intento de confirmar pago efectivo en estado inválido. ComandaId: {ComandaId}, Estado: {Estado}", comanda.Id, comanda.Estado);
                throw new ArgumentException("La comanda no está esperando pago.");
            }

            Pago pagoExistente = await _pagoRepositorio.ObtenerPagoPorComandaIdAsync(comanda.Id);
            if (pagoExistente != null && pagoExistente.EstadoPago == EstadoPago.Confirmado)
            {
                _logger.LogWarning("Intento de confirmar pago ya confirmado (idempotencia). ComandaId: {ComandaId}", comanda.Id);
                throw new InvalidOperationException("El pago ya fue confirmado");
            }
        }

        private async Task FinalizarComandaYNotificarAsync(Comanda comanda)
        {
            comanda.Estado = EstadoComanda.Finalizada;
            comanda.HoraFin = DateTime.Now;
            await _comandaRepositorio.ActualizarAsync(comanda);

            await _comandaNotificador.NotificarEstadoModificadoAsync(comanda, new List<int> { comanda.MozoId.Value });

            await _llamadoRepositorio.ResolverLlamadoPorMesaYCategoriaAsync(comanda.MesaId, (int)CategoriaLlamado.Pago);
        }
    }
}
