using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.CasosDeUso.PlatoCasosDeUso
{
    public class ModificarPlatoCasoDeUso
    {
        private readonly IPlatoRepositorio _platoRepositorio;
        private readonly IImagenServicio _servicioImagen;

        public ModificarPlatoCasoDeUso(IPlatoRepositorio platoRepositorio, IImagenServicio servicioImagen)
        {
            _platoRepositorio = platoRepositorio;
            _servicioImagen = servicioImagen;
        }

        public async Task<Plato> EjecutarAsync(int restauranteId, Plato platoModificado, string carpetaCloudinary, Stream stream, string nombreImagen)
        {

            var platoExistente = await _platoRepositorio.ObtenerPorIdAsync(platoModificado.Id, restauranteId);
            if (platoExistente == null)
            {
                throw new ArgumentException("El plato que intenta modificar no existe o no pertenece al restaurante.");
            }

            platoExistente.Nombre = platoModificado.Nombre;
            platoExistente.Descripcion = platoModificado.Descripcion;
            platoExistente.PrecioVentaFinal = platoModificado.PrecioVentaFinal;
            platoExistente.TiempoPreparacionBase = platoModificado.TiempoPreparacionBase;
            platoExistente.TipoPlatoId = platoModificado.TipoPlatoId;
            platoExistente.CategoriaPlatoId = platoModificado.CategoriaPlatoId;
            platoExistente.EsVisibleEnCarta = platoModificado.EsVisibleEnCarta;

            platoExistente.Restricciones = platoModificado.Restricciones;
            platoExistente.Ingredientes = platoModificado.Ingredientes;
            platoExistente.EsPrecioManual = platoModificado.EsPrecioManual;

            if (stream != null && !string.IsNullOrEmpty(nombreImagen))
            {
                platoExistente.UrlImagen = await _servicioImagen.SubirImagenAsync(stream, nombreImagen, carpetaCloudinary);
            }

            await _platoRepositorio.ActualizarAsync(platoExistente);

            return platoExistente;
        }
    }
}
