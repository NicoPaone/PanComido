using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces;
using PanComido.Dominio.Interfaces.Repositorios;
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

        public ListarInsumoCasoDeUso(IInsumoRepositorio insumoRepositorio, ILoteRepositorio loteRepositorio, IEstadoStockInsumoServicio estadoStockInsumoServicio)
        {
            _insumoRepositorio = insumoRepositorio;
            _loteRepositorio = loteRepositorio;
            _estadoStockInsumoServicio = estadoStockInsumoServicio;
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
                    .CalcularEstadoStock(insumo.StockActual, insumo.StockMinimo);
            }

            return insumos;
        }
    }
}
