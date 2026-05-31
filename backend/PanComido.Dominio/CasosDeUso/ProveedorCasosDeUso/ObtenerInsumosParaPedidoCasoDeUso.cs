using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            decimal cantidadSugerida = 0;

            var insumosProveedor = await _insumoRepositorio.ObtenerInsumosDelProveedorAsync(proveedorId, restauranteId);
            var insumosResto = await _insumoRepositorio.ObtenerInsumosAsync(restauranteId);

            var insumosConSugerencia = new List<InsumoConSugerencia>();
            
            foreach (var insumo in insumosResto)
            {
              if(insumo.Id == 0 || insumosProveedor.All(i => i.Id != insumo.Id)) continue;

                decimal stockActualInsumo = await _loteRepositorio.ObtenerStockTotalDeInsumo(insumo.Id);

                var estadoStock = _estadoStockInsumoServicio.CalcularEstadoStock(stockActualInsumo, insumo.StockMinimo);
                if (estadoStock == EstadoStock.Critico) cantidadSugerida = insumo.StockMinimo * 2;
                else if (estadoStock == EstadoStock.Bajo) cantidadSugerida = insumo.StockMinimo;
                else continue;

                decimal ultimoPrecioCompra = await _pedidoRepositorio.ObtenerUltimoPrecioCompraUnitarioAsync(insumo.Id, proveedorId);
                decimal calcularPrecioTotal = cantidadSugerida * ultimoPrecioCompra;

                insumosConSugerencia.Add(new InsumoConSugerencia
                    {
                        Id = insumo.Id,
                        Nombre = insumo.Nombre,
                        UnidadMedida = insumo.UnidadMedida,
                        StockActual = stockActualInsumo,
                        CantidadSugerida = cantidadSugerida,
                        EstadoStock = estadoStock.ToString(),
                        PrecioUnitario = ultimoPrecioCompra,
                        PrecioTotalSugerido = calcularPrecioTotal
                });
            }
            return insumosConSugerencia;
        }
    }
}
