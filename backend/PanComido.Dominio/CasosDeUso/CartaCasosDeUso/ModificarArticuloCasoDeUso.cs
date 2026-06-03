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

        // Agregamos el restauranteId como primer parámetro
        public async Task EjecutarAsync(int restauranteId, int articuloId, bool? visibleEnCarta, bool? destacado)
        {
            // Ahora usamos el ID real que viene de la sesión
            var articulo = await _articuloRepositorio.ObtenerDetalleAsync(restauranteId, articuloId);

            if (articulo == null) return;

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