using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso
{
    public class ListarInsumoCasoDeUso
    {
        private readonly IInsumoRepositorio _insumoRepositorio;
        private readonly ILoteRepositorio _loteRepositorio;

        public ListarInsumoCasoDeUso(IInsumoRepositorio insumoRepositorio, ILoteRepositorio loteRepositorio)
        {
            _insumoRepositorio = insumoRepositorio;
            _loteRepositorio = loteRepositorio;
        }

        public async Task<List<Insumo>> EjecutarAsync(
            int restauranteId,
            string? filtroCategoria = null,
            string? busqueda = null)
        {
            List<Insumo> insumos;

            insumos = await _insumoRepositorio.ObtenerInsumosAsync(restauranteId);
            foreach (var insumo in insumos)
            {
                decimal stockTotal = await _loteRepositorio.ObtenerStockTotalDeInsumo(insumo.Id);
                insumo.StockActual = stockTotal;

                
                if (stockTotal < insumo.StockMinimo)
                    insumo.EstadoStock = Entidades.Enums.EstadoStock.Critico;
                else if (stockTotal < (insumo.StockMinimo * 2))
                    insumo.EstadoStock = Entidades.Enums.EstadoStock.Bajo;
                else
                    insumo.EstadoStock = Entidades.Enums.EstadoStock.Normal;

            }

            return insumos;
        }
    }
}
