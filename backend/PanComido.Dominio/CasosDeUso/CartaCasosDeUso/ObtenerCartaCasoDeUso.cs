using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
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

        private readonly IDisponibilidadArticuloServicio _disponibilidadServicio;

        public ObtenerCartaCasoDeUso(
            IArticuloRepositorio articuloRepositorio,
            ILoteRepositorio loteRepositorio,
            IDisponibilidadArticuloServicio disponibilidadServicio)
        {
            _articuloRepositorio = articuloRepositorio;
            _loteRepositorio = loteRepositorio;
            _disponibilidadServicio = disponibilidadServicio;
        }

        public async Task<List<Articulo>> EjecutarAsync(int restauranteId)
        {
            List<Articulo> articulosEnCarta = await _articuloRepositorio.ObtenerArticulosEnCartaConIngredientesAsync(restauranteId);
            Dictionary<int, decimal> stockDeInsumosActual = await _loteRepositorio.ObtenerStockTotalDeInsumosDisponible(restauranteId, DateOnly.FromDateTime(DateTime.UtcNow));

            List<Articulo> articulosDisponiblesEnCarta = new List<Articulo>();

            foreach (Articulo articulo in articulosEnCarta)
            {
                if (_disponibilidadServicio.VerificarDisponibilidad(articulo, stockDeInsumosActual))
                {
                    articulosDisponiblesEnCarta.Add(articulo);
                }
            }

            return articulosDisponiblesEnCarta;
        }
    }
}
