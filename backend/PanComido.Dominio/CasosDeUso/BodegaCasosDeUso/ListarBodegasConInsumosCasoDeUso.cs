using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.BodegaCasosDeUso
{
    public class ListarBodegasConInsumosCasoDeUso
    {
        private readonly IBodegaRepositorio _bodegaRepositorio;
        private readonly ILoteRepositorio _loteRepositorio;
        private readonly IInsumoRepositorio _insumoRepositorio;
        private readonly IEstadoStockInsumoServicio _estadoStockInsumoServicio;

        public ListarBodegasConInsumosCasoDeUso(IBodegaRepositorio bodegaRepositorio, ILoteRepositorio loteRepositorio, IInsumoRepositorio insumoRepositorio, IEstadoStockInsumoServicio estadoStockInsumoServicio)
        {
            _bodegaRepositorio = bodegaRepositorio;
            _loteRepositorio = loteRepositorio;
            _insumoRepositorio = insumoRepositorio;
            _estadoStockInsumoServicio = estadoStockInsumoServicio;
        }

        public async Task<List<Bodega>> EjecutarAsync(int restauranteId)
        {
            // obtengo bodegas con su lista de insumos vacia
            List<Bodega> bodegas = await _bodegaRepositorio.ObtenerBodegasAsync(restauranteId);
            // obtengo insumos del restaurante 
            List<Insumo> insumos = await _insumoRepositorio.ObtenerInsumosAsync(restauranteId);

            // obtengo el stock de los insumos por bodega
            Dictionary<(int insumoId, int bodegaId), decimal> stockDeInsumoPorBodega = await _loteRepositorio.ObtenerStocksPorBodega(restauranteId);
            // obtengo el vencimiento mas proximo de los insumos por bodega
            Dictionary<(int insumoId, int bodegaId), DateOnly?> vencimientosMasProximosDeInsumoPorBodega = await _loteRepositorio.ObtenerVencimientosPorBodega(restauranteId);

            foreach (Bodega bodega in bodegas)
            {

                bodega.Insumos = new List<Insumo>();

                foreach (Insumo insumo in insumos)
                {
                    var key = (insumo.Id, bodega.Id);

                    if (stockDeInsumoPorBodega.ContainsKey(key))
                    {
                        Insumo insumoConStockTotalYFechaDeVencimiento =
                            new Insumo
                            {
                                Id = insumo.Id,
                                Nombre = insumo.Nombre,
                                Tipo = insumo.Tipo,
                                UnidadMedida = insumo.UnidadMedida,
                                Categoria = insumo.Categoria,
                                StockMinimo = insumo.StockMinimo,
                                StockActual = stockDeInsumoPorBodega.GetValueOrDefault(key, 0m),
                                Vencimiento = vencimientosMasProximosDeInsumoPorBodega.GetValueOrDefault(key, null)
                            };

                        insumoConStockTotalYFechaDeVencimiento.EstadoStock = _estadoStockInsumoServicio
                            .CalcularEstadoStock(insumoConStockTotalYFechaDeVencimiento.StockActual, insumoConStockTotalYFechaDeVencimiento.StockMinimo);

                        bodega.Insumos.Add(insumoConStockTotalYFechaDeVencimiento);
                    }
                }
            }

            return bodegas;
        }
    }
}
