using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.CasosDeUso.BebidaPreparadaCasosDeUso
{
    public class ModificarBebidaPreparadaCasoDeUso
    {
        private readonly IBebidaPreparadaRepositorio _bebidaPreparadaRepositorio;
        private readonly IInsumoValidacionServicio _insumoValidacionServicio;
        private readonly IBebidaPreparadaValidacionServicio _bebidaPreparadaValidacionServicio;
        private readonly IImagenServicio _imagenServicio;
        private readonly INormalizadorNombreServicio _normalizadorNombreServicio;
        private readonly ILogger<ModificarBebidaPreparadaCasoDeUso> _logger;

        public ModificarBebidaPreparadaCasoDeUso(
            IBebidaPreparadaRepositorio bebidaPreparadaRepositorio,
            IInsumoValidacionServicio insumoValidacionServicio,
            IBebidaPreparadaValidacionServicio bebidaPreparadaValidacionServicio,
            IImagenServicio imagenServicio,
            INormalizadorNombreServicio normalizadorNombreServicio,
            ILogger<ModificarBebidaPreparadaCasoDeUso> logger)
        {
            _bebidaPreparadaRepositorio = bebidaPreparadaRepositorio;
            _insumoValidacionServicio = insumoValidacionServicio;
            _bebidaPreparadaValidacionServicio = bebidaPreparadaValidacionServicio;
            _imagenServicio = imagenServicio;
            _normalizadorNombreServicio = normalizadorNombreServicio;
            _logger = logger;
        }

        public async Task<BebidaPreparada> EjecutarAsync(int restauranteId, BebidaPreparada bebidaPreparada, string carpetaCloudinary, Stream stream, string nombreImagen)
        {
            _logger.LogInformation("Iniciando modificación de la bebida preparada {Id} para el restaurante {RestauranteId}.", bebidaPreparada.Id, restauranteId);

            _bebidaPreparadaValidacionServicio.ValidarDatosBasicos(bebidaPreparada);

            bebidaPreparada.Nombre = _normalizadorNombreServicio.Normalizar(bebidaPreparada.Nombre);

            var bebidaExistente = await _bebidaPreparadaRepositorio.ObtenerPorIdAsync(bebidaPreparada.Id, restauranteId);
            if (bebidaExistente == null)
            {
                _logger.LogWarning("Rechazo al modificar bebida preparada: La bebida preparada {Id} no existe o no pertenece al restaurante {RestauranteId}.", bebidaPreparada.Id, restauranteId);
                throw new KeyNotFoundException("La bebida preparada no existe o no pertenece al restaurante.");
            }

            await ValidarNombreDuplicadoAsync(restauranteId, bebidaPreparada.Nombre, bebidaExistente.Nombre);

            await _insumoValidacionServicio.ValidarInsumosDeRecetaBebidaAsync(restauranteId, bebidaPreparada.Insumos);

            bebidaPreparada.RestauranteId = restauranteId;
            bebidaPreparada.UrlImagen = bebidaExistente.UrlImagen;

            if (stream != null && !string.IsNullOrEmpty(nombreImagen))
            {
                bebidaPreparada.UrlImagen = await _imagenServicio.SubirImagenAsync(stream, nombreImagen, carpetaCloudinary);
            }

            BebidaPreparada bebidaActualizada = await _bebidaPreparadaRepositorio.ActualizarAsync(bebidaPreparada);

            _logger.LogInformation("Bebida preparada '{Nombre}' (ID {Id}) modificada exitosamente en el restaurante {RestauranteId}.", bebidaActualizada.Nombre, bebidaActualizada.Id, restauranteId);

            return bebidaActualizada;
        }

        private async Task ValidarNombreDuplicadoAsync(int restauranteId, string nombreNuevo, string nombreActual)
        {
            bool elNombreCambio = !string.Equals(nombreNuevo, nombreActual, StringComparison.OrdinalIgnoreCase);
            if (elNombreCambio && await _bebidaPreparadaRepositorio.ExisteBebidaPreparadaConNombreAsync(restauranteId, nombreNuevo))
            {
                _logger.LogWarning("Rechazo al modificar bebida preparada: Ya existe una con el nombre '{Nombre}' en el restaurante {RestauranteId}.", nombreNuevo, restauranteId);
                throw new ArgumentException($"Ya existe una bebida preparada con el nombre '{nombreNuevo}' en el restaurante.");
            }
        }
    }
}
