using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso
{
    public class ObtenerInsumosParaPedidoCasoDeUso
    {
        private readonly IInsumoRepositorio _insumoRepositorio;
        private readonly IEstadoStockInsumoServicio _estadoStockInsumoServicio;
        private readonly ILoteRepositorio _loteRepositorio;
        private readonly IPedidoRepositorio _pedidoRepositorio;

        public ObtenerInsumosParaPedidoCasoDeUso(IInsumoRepositorio insumoRepositorio, IEstadoStockInsumoServicio estadoStockInsumoServicio, ILoteRepositorio loteRepositorio, IPedidoRepositorio pedidoRepositorio)
        {
            _insumoRepositorio = insumoRepositorio;
            _estadoStockInsumoServicio = estadoStockInsumoServicio;
            _loteRepositorio = loteRepositorio;
            _pedidoRepositorio = pedidoRepositorio;
        }

        public async Task<List<InsumoConSugerencia>> EjecutarAsync(int proveedorId, int restauranteId)
        {
            var insumosProveedor = await _insumoRepositorio.ObtenerInsumosDelProveedorAsync(proveedorId, restauranteId);
            var insumosResto = await _insumoRepositorio.ObtenerInsumosAsync(restauranteId);
            List<int> insumoEnPedidoPendiente = await _pedidoRepositorio.ObtenerInsumosEnPedidosNoRecibidosAsync(proveedorId);

            return await FiltrarInsumosBajoStockMinimoAsync(insumosResto, insumosProveedor, insumoEnPedidoPendiente, proveedorId);
        }

        private async Task<List<InsumoConSugerencia>> FiltrarInsumosBajoStockMinimoAsync(
            List<Insumo> insumosResto,
            List<Insumo> insumosProveedor,
            List<int> insumoEnPedidoPendiente,
            int proveedorId)
        {
            var insumosConSugerencia = new List<InsumoConSugerencia>();

            foreach (var insumo in insumosResto)
            {
                if (insumo.Id == 0 || insumosProveedor.All(i => i.Id != insumo.Id)) continue;
                if (insumoEnPedidoPendiente.Contains(insumo.Id)) continue;

                decimal stockActualInsumo = await _loteRepositorio.ObtenerStockTotalDeInsumo(insumo.Id);
                var estadoStock = _estadoStockInsumoServicio.CalcularEstadoStock(stockActualInsumo, insumo.StockMinimo);

                decimal cantidadSugerida;
                if (estadoStock == EstadoStock.Critico) cantidadSugerida = insumo.StockMinimo * 2;
                else if (estadoStock == EstadoStock.Bajo) cantidadSugerida = insumo.StockMinimo;
                else continue;

                decimal ultimoPrecioCompra = await _pedidoRepositorio.ObtenerUltimoPrecioCompraUnitarioAsync(insumo.Id, proveedorId);

                insumosConSugerencia.Add(new InsumoConSugerencia
                {
                    Id = insumo.Id,
                    Nombre = insumo.Nombre,
                    UnidadMedida = insumo.UnidadMedida,
                    StockActual = stockActualInsumo,
                    CantidadSugerida = cantidadSugerida,
                    EstadoStock = estadoStock.ToString(),
                    PrecioUnitario = ultimoPrecioCompra,
                    PrecioTotalSugerido = cantidadSugerida * ultimoPrecioCompra
                });
            }
            return insumosConSugerencia;
        }
    }
}
