using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.AvisosCasosDeUso
{
    public class ListarInsumosConStockCriticoCasoDeUso
    {
        private readonly IInsumoRepositorio _insumoRepositorio;
        private readonly IEstadoStockInsumoServicio _estadoStockInsumoServicio;
        private readonly ILoteRepositorio _loteRepositorio;

        public ListarInsumosConStockCriticoCasoDeUso(IInsumoRepositorio insumoRepositorio, 
                                            IEstadoStockInsumoServicio estadoStockInsumoServicio,
                                            ILoteRepositorio loteRepositorio)
        {
            _insumoRepositorio = insumoRepositorio;
            _estadoStockInsumoServicio = estadoStockInsumoServicio;
            _loteRepositorio = loteRepositorio;
        }

        public async Task<List<Insumo>> EjecutarAsync(int restauranteId)
        {
            List<Insumo> insumos = await _insumoRepositorio.ObtenerInsumosAsync(restauranteId);
            List<Insumo> insumosConStockCritico = new List<Insumo>();
            var stockDisponible = await _loteRepositorio.ObtenerStockTotalDeInsumosDisponible(restauranteId, DateOnly.FromDateTime(DateTime.UtcNow));

            foreach (var insumo in insumos)
            {
                insumo.StockActual = stockDisponible.TryGetValue(insumo.Id, out var stock) ? stock : 0m;
                insumo.Vencimiento = await _loteRepositorio.ObtenerFechaDeVencimientoMasProximaDeInsumo(insumo.Id);

                if (_estadoStockInsumoServicio.CalcularEstadoStock(insumo.StockActual, insumo.StockMinimo, insumo.StockRecomendado) == EstadoStock.Critico)
                {
                    insumo.EstadoStock = EstadoStock.Critico;
                    insumosConStockCritico.Add(insumo);
                }
            }
            return insumosConStockCritico;
        }
    }
}