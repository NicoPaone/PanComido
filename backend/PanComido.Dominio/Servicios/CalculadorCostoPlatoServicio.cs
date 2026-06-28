using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.Servicios
{
    public class CalculadorCostoPlatoServicio : ICalculadorCostoPlatoServicio
    {
        private readonly IPlatoAnalisisRepositorio _platoAnalisisRepositorio;

        public CalculadorCostoPlatoServicio(IPlatoAnalisisRepositorio platoAnalisisRepositorio)
        {
            _platoAnalisisRepositorio = platoAnalisisRepositorio;
        }

        public async Task<decimal> CalcularCostoAsync(Plato plato)
        {
            if (plato == null || plato.Ingredientes == null || !plato.Ingredientes.Any())
            {
                return 0m;
            }

            var insumoIds = plato.Ingredientes.Select(i => i.InsumoId).Distinct().ToList();
            var precios = await _platoAnalisisRepositorio.ObtenerUltimosPreciosCompraInsumosAsync(insumoIds);

            decimal costoPreparacion = 0m;
            foreach (var item in plato.Ingredientes)
            {
                precios.TryGetValue(item.InsumoId, out decimal ultimoPrecioCompra);
                costoPreparacion += item.Cantidad * ultimoPrecioCompra;
            }

            return costoPreparacion;
        }
    }
}
