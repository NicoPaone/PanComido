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

        public ObtenerInsumosParaPedidoCasoDeUso(IInsumoRepositorio insumoRepositorio, IEstadoStockInsumoServicio estadoStockInsumoServicio)
        {
            _insumoRepositorio = insumoRepositorio;
            _estadoStockInsumoServicio = estadoStockInsumoServicio;
        }

        public async Task<List<InusmoConSugerencia>> EjecutarAsync(int proveedorId, int restauranteId)
        {
            decimal cantidadSugerida = 0;

            var insumos = await _insumoRepositorio.ObtenerInsumosDelProveedorAsync(proveedorId, restauranteId);
            var insumosConSugerencia = new List<InusmoConSugerencia>();

            foreach (var insumo in insumos)
            {
                var estadoStock = _estadoStockInsumoServicio.CalcularEstadoStock(insumo.StockActual, insumo.StockMinimo);
                if (estadoStock == EstadoStock.Critico) cantidadSugerida = insumo.StockMinimo * 2;
                else if (estadoStock == EstadoStock.Bajo) cantidadSugerida = insumo.StockMinimo;
                else continue;

                insumosConSugerencia.Add(new InusmoConSugerencia
                    {
                        Id = insumo.Id,
                        Nombre = insumo.Nombre,
                        UnidadMedida = insumo.UnidadMedida,
                        StockActual = insumo.StockActual,
                        CantidadSugerida = cantidadSugerida,
                        EstadoStock = estadoStock.ToString()
                    });
            }
            return insumosConSugerencia;
        }
    }
}
