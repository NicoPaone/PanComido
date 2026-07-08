using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Dominio.CasosDeUso.PedidosCasosDeUso
{
    public class RecibirPedidoProveedorCasoDeUso
    {
        private readonly IPedidoRepositorio _pedidoRepositorio;
        private readonly ILoteRepositorio _loteRepositorio;
        private readonly IBodegaRepositorio _bodegaRepositorio;
        private readonly ILogger<RecibirPedidoProveedorCasoDeUso> _logger;

        public RecibirPedidoProveedorCasoDeUso(
            IPedidoRepositorio pedidoRepositorio,
            ILoteRepositorio loteRepositorio,
            IBodegaRepositorio bodegaRepositorio,
            ILogger<RecibirPedidoProveedorCasoDeUso> logger)
        {
            _pedidoRepositorio = pedidoRepositorio;
            _loteRepositorio = loteRepositorio;
            _bodegaRepositorio = bodegaRepositorio;
            _logger = logger;
        }

        public async Task EjecutarAsync(int pedidoId, List<DOM.Lote> lotesAGuardar, List<DOM.PedidoInsumo> itemsConPrecioConfirmado, int restauranteId)
        {
            DOM.Pedido pedido = await _pedidoRepositorio.ObtenerPedidoPorIdAsync(pedidoId);
            if (pedido == null) throw new KeyNotFoundException("Pedido no encontrado");
            if (pedido.Estado != EstadoPedidoProveedor.Enviado) throw new InvalidOperationException("Solo se pueden recibir pedidos en estado Enviado");

            await ValidarLotesAsync(lotesAGuardar, restauranteId);

            if(pedido.ItemsInsumo.Count != itemsConPrecioConfirmado.Count)
                throw new InvalidOperationException("La cantidad de items con precio confirmado no coincide con la cantidad de items del pedido.");

            await _loteRepositorio.CrearLotesAsync(lotesAGuardar);
            await _pedidoRepositorio.MarcarComoRecibidoAsync(pedidoId, itemsConPrecioConfirmado);

            _logger.LogInformation("Pedido recibido. PedidoId: {PedidoId}", pedidoId);
        }

        private async Task ValidarLotesAsync(List<Lote> lotesAGuardar, int restauranteId)
        {
            foreach (var lote in lotesAGuardar)
            {
                if (lote.FechaVencimiento <= DateOnly.FromDateTime(DateTime.Today))
                    throw new ArgumentException($"La fecha de vencimiento del lote {lote.Nombre} debe ser una fecha futura.");
                if (lote.Cantidad <= 0)
                    throw new ArgumentException($"La cantidad del lote {lote.Nombre} debe ser mayor a cero.");
                if (!await _bodegaRepositorio.ExisteBodegaEnRestauranteAsync(restauranteId, lote.BodegaId))
                    throw new ArgumentException($"La bodega del lote {lote.Nombre} no es válida.");
            }
        }
    }
}
