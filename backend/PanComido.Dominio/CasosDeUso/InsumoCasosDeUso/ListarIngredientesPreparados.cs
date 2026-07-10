using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.CasosDeUso.InsumoCasosDeUso
{
    public class ListarIngredientesPreparados
    {
        private readonly IIngredientePreparadoRepositorio _ingredientePreparadoRepositorio;
        private readonly ILoteRepositorio _loteRepositorio;
        private readonly IEstadoStockInsumoServicio _estadoStockServicio;

        public ListarIngredientesPreparados(IIngredientePreparadoRepositorio ingredientePreparadoRepositorio, ILoteRepositorio loteRepositorio, IEstadoStockInsumoServicio estadoStockServicio)
        {
            _ingredientePreparadoRepositorio = ingredientePreparadoRepositorio;
            _loteRepositorio = loteRepositorio;
            _estadoStockServicio = estadoStockServicio;
        }

            public async Task<List <Entidades.IngredientePreparado>> Ejecutar (int restauranteId)

        {
            var ingredientesPreparados = await _ingredientePreparadoRepositorio.ObtenerTodosAsync(restauranteId);

            foreach (var ingrediente in ingredientesPreparados)
            {
                ingrediente.StockActual  = await _loteRepositorio.ObtenerStockTotalDeInsumo(ingrediente.Id);

                ingrediente.FechaVencimientoProxima = (DateOnly)await _loteRepositorio.ObtenerFechaDeVencimientoMasProximaDeInsumo(ingrediente.Id);

                ingrediente.EstadoStock =  _estadoStockServicio.CalcularEstadoStock(ingrediente.StockActual, ingrediente.StockMinimo, ingrediente.StockRecomendado);

            }

            return ingredientesPreparados;
        }

    }
}
