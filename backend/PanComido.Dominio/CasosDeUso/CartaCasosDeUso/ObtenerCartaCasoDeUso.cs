using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.CartaCasosDeUso
{
    public class ObtenerCartaCasoDeUso
    {
        private readonly IArticuloRepositorio _articuloRepositorio;
        private readonly ILoteRepositorio _loteRepositorio;
        public ObtenerCartaCasoDeUso(IArticuloRepositorio articuloRepositorio, ILoteRepositorio loteRepositorio)
        {
            _articuloRepositorio = articuloRepositorio;
            _loteRepositorio = loteRepositorio;
        }

        public async Task<List<Articulo>> EjecutarAsync(int restauranteId)
        {

                List<Articulo> articulosEnCarta = await _articuloRepositorio.ObtenerArticulosEnCartaConIngredientesAsync(restauranteId);
                Dictionary<int, decimal> stockDeInsumosActual = await _loteRepositorio.ObtenerStockTotalDeInsumos(restauranteId);

                List<Articulo> articulosDisponiblesEnCarta = new List<Articulo>();

                foreach (Articulo articulo in articulosEnCarta)
                {
                    bool estaDisponible = false;

                    if (articulo is Plato plato)
                    {
                        estaDisponible = plato.Ingredientes
                            .Where(i => !i.Opcional)
                            .All(i => stockDeInsumosActual.TryGetValue(i.InsumoId, out decimal stock) && stock >= i.Cantidad);
                    }
                    else // es bebida por descarte
                        estaDisponible = stockDeInsumosActual.TryGetValue(articulo.Id, out decimal stock) && stock > 0;

                    if (estaDisponible)
                        articulosDisponiblesEnCarta.Add(articulo);
                }

                return articulosDisponiblesEnCarta;
        }
    }
}
