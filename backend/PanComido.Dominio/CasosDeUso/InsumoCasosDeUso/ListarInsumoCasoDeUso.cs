using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.InsumoCasosDeUso
{
    public class ListarInsumoCasoDeUso
    {
        private readonly IInsumoRepositorio _insumoRepositorio;
        private readonly ILoteRepositorio _loteRepositorio;

        private readonly IEstadoStockInsumoServicio _estadoStockInsumoServicio;
        private readonly IUltimoPrecioCompraInsumoServicio _ultimoPrecioCompraServicio;

        public ListarInsumoCasoDeUso(
            IInsumoRepositorio insumoRepositorio,
            ILoteRepositorio loteRepositorio,
            IEstadoStockInsumoServicio estadoStockInsumoServicio,
            IUltimoPrecioCompraInsumoServicio ultimoPrecioCompraServicio)
        {
            _insumoRepositorio = insumoRepositorio;
            _loteRepositorio = loteRepositorio;
            _estadoStockInsumoServicio = estadoStockInsumoServicio;
            _ultimoPrecioCompraServicio = ultimoPrecioCompraServicio;
        }

        public async Task<List<Insumo>> EjecutarAsync(int restauranteId)
        {
            List<Insumo> insumos;

            insumos = await _insumoRepositorio.ObtenerInsumosAsync(restauranteId);
            foreach (var insumo in insumos)
            {
                insumo.StockActual = await _loteRepositorio.ObtenerStockTotalDeInsumo(insumo.Id);
                insumo.Vencimiento = await _loteRepositorio.ObtenerFechaDeVencimientoMasProximaDeInsumo(insumo.Id);


                insumo.EstadoStock = _estadoStockInsumoServicio
                    .CalcularEstadoStock(insumo.StockActual, insumo.StockMinimo, insumo.StockRecomendado);

                insumo.CostoCalculado = _ultimoPrecioCompraServicio.ObtenerUltimoPrecioCompraRecibido(insumo.PedidoInsumos);
            }

            return insumos;
        }
    }
}
