using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections;
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
            List<Bodega> bodegas = await _bodegaRepositorio.ObtenerBodegasAsync(restauranteId);
            List<Insumo> insumos = await _insumoRepositorio.ObtenerInsumosAsync(restauranteId);

            Dictionary<(int insumoId, int bodegaId), decimal> stockDeInsumoPorBodega = await _loteRepositorio.ObtenerStocksPorBodega(restauranteId);

            Dictionary<(int insumoId, int bodegaId), DateOnly?> vencimientosMasProximosDeInsumoPorBodega = await _loteRepositorio.ObtenerVencimientosPorBodega(restauranteId);

            AsignarInsumosABodegas(bodegas, insumos, stockDeInsumoPorBodega, vencimientosMasProximosDeInsumoPorBodega);

            return bodegas;
        }

        private void AsignarInsumosABodegas(
            List<Bodega> bodegas,
            List<Insumo> insumos,
            Dictionary<(int, int), decimal> stocks,
            Dictionary<(int, int), DateOnly?> vencimientos)
        {
            foreach (Bodega bodega in bodegas)
            {
                bodega.Insumos = new List<Insumo>();
                foreach (Insumo insumo in insumos)
                {
                    var key = (insumo.Id, bodega.Id);
                    if (stocks.ContainsKey(key))
                    {
                        Insumo insumoEnsamblado = ConstruirInsumoParaBodega(insumo, key, stocks, vencimientos);
                        bodega.Insumos.Add(insumoEnsamblado);
                    }
                }
            }
        }

        private Insumo ConstruirInsumoParaBodega(
            Insumo insumoOriginal,
            (int, int) keyDictionary,
            Dictionary<(int, int), decimal> stocks,
            Dictionary<(int, int), DateOnly?> vencimientos)
        {
            Insumo nuevoInsumo = new Insumo
            {
                Id = insumoOriginal.Id,
                Nombre = insumoOriginal.Nombre,
                Tipo = insumoOriginal.Tipo,
                UnidadMedida = insumoOriginal.UnidadMedida,
                Categoria = insumoOriginal.Categoria,
                StockMinimo = insumoOriginal.StockMinimo,
                StockActual = stocks.GetValueOrDefault(keyDictionary, 0m),
                Vencimiento = vencimientos.GetValueOrDefault(keyDictionary, null)
            };

            nuevoInsumo.EstadoStock = _estadoStockInsumoServicio.CalcularEstadoStock(nuevoInsumo.StockActual, nuevoInsumo.StockMinimo);
            return nuevoInsumo;
        }
    }
}
