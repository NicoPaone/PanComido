using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso
{
    public class ListarInsumosDelProveedorCasoDeUso
    {

        private readonly IInsumoRepositorio _insumoRepositorio;

        public ListarInsumosDelProveedorCasoDeUso(IInsumoRepositorio insumoRepositorio)
        {
            _insumoRepositorio = insumoRepositorio;
        }

        public async Task<List<Insumo>> EjecutarAsync(int proveedorId, int restauranteId)
        {

            List<Insumo> insumos = await _insumoRepositorio.ObtenerInsumosDelProveedorAsync(proveedorId, restauranteId);

            foreach (var insumo in insumos)
            {
                if (insumo.StockActual < insumo.StockMinimo)
                {
                    insumo.EstadoStock = EstadoStock.Critico;
                }
                else if (insumo.StockActual < insumo.StockMinimo * 2)
                {
                    insumo.EstadoStock = EstadoStock.Bajo;
                }
                else
                {
                    insumo.EstadoStock = EstadoStock.Normal;
                }
            }
            return insumos;
        }
    }
}
