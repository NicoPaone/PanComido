using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.CartaCasosDeUso
{
    public class ModificarArticuloCasoDeUso
    {
        private readonly IArticuloRepositorio _articuloRepositorio;

        public ModificarArticuloCasoDeUso(IArticuloRepositorio articuloRepositorio)
        {
            _articuloRepositorio = articuloRepositorio;
        }

        public async Task EjecutarAsync(int restauranteId, int articuloId, bool? visibleEnCarta, bool? destacado)
        {
            var articulo = await _articuloRepositorio.ObtenerDetalleAsync(restauranteId, articuloId);

            if (articulo == null)
            {
                throw new System.ArgumentException("El artículo que intenta modificar no existe o no pertenece al restaurante.");
            }

            if (visibleEnCarta.HasValue)
            {
                articulo.EsVisibleEnCarta = visibleEnCarta.Value;
            }

            if (destacado.HasValue && articulo is Plato plato)
            {
                plato.Destacado = destacado.Value;
            }

            await _articuloRepositorio.ActualizarAsync(articulo);
        }
    }
}