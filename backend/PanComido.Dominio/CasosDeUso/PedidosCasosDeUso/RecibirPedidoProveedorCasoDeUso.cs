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

        public RecibirPedidoProveedorCasoDeUso(
            IPedidoRepositorio pedidoRepositorio,
            ILoteRepositorio loteRepositorio,
            IBodegaRepositorio bodegaRepositorio)
        {
            _pedidoRepositorio = pedidoRepositorio;
            _loteRepositorio = loteRepositorio;
            _bodegaRepositorio = bodegaRepositorio;
        }

        public async Task EjecutarAsync(int pedidoId, List<DOM.Lote> lotesAGuardar, int restauranteId)
        {
            DOM.Pedido pedido = await _pedidoRepositorio.ObtenerPedidoPorIdAsync(pedidoId);
            if (pedido == null) throw new KeyNotFoundException("Pedido no encontrado");
            if (pedido.Estado != EstadoPedidoProveedor.Enviado) throw new InvalidOperationException("Solo se pueden recibir pedidos en estado Enviado");

            await ValidarLotesAsync(lotesAGuardar, restauranteId);
            await _loteRepositorio.CrearLotesAsync(lotesAGuardar);
            await _pedidoRepositorio.MarcarComoRecibidoAsync(pedidoId);
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
