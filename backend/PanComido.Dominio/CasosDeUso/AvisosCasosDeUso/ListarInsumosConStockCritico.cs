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
    public class ListarInsumosConStockCritico
    {
        private readonly IInsumoRepositorio _insumoRepositorio;
        private readonly IEstadoStockInsumoServicio _estadoStockInsumoServicio;
        private readonly ILoteRepositorio _loteRepositorio;

        public ListarInsumosConStockCritico(IInsumoRepositorio insumoRepositorio, 
                                            IEstadoStockInsumoServicio estadoStockInsumoServicio,
                                            ILoteRepositorio loteRepositorio)
        {
            _insumoRepositorio = insumoRepositorio;
            _estadoStockInsumoServicio = estadoStockInsumoServicio;
            _loteRepositorio = loteRepositorio;
        }

        public async Task<List<Insumo>> Ejecutar(int restauranteId)
        {
            List<Insumo> insumos = await _insumoRepositorio.ObtenerInsumosAsync(restauranteId);
            List<Insumo> insumosConStockCritico = new List<Insumo>();
            foreach (var insumo in insumos)
            {
                decimal stockActualInsumo = await _loteRepositorio.ObtenerStockTotalDeInsumo(insumo.Id);
                if (_estadoStockInsumoServicio.CalcularEstadoStock(stockActualInsumo, insumo.StockMinimo) == EstadoStock.Critico)
                {
                    insumo.EstadoStock = EstadoStock.Critico;
                    insumosConStockCritico.Add(insumo);
                }
            }
            return insumosConStockCritico;
        }
    }
}