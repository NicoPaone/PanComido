using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.CasosDeUso.BebidaPreparadaCasosDeUso
{
    public class CrearBebidaPreparadaCasoDeUso
    {
        private readonly IBebidaPreparadaRepositorio _bebidaPreparadaRepositorio;
        private readonly IInsumoValidacionServicio _insumoValidacionServicio;
        private readonly IBebidaPreparadaValidacionServicio _bebidaPreparadaValidacionServicio;
        private readonly IImagenServicio _imagenServicio;
        private readonly INormalizadorNombreServicio _normalizadorNombreServicio;
        private readonly ILogger<CrearBebidaPreparadaCasoDeUso> _logger;

        public CrearBebidaPreparadaCasoDeUso(
            IBebidaPreparadaRepositorio bebidaPreparadaRepositorio,
            IInsumoValidacionServicio insumoValidacionServicio,
            IBebidaPreparadaValidacionServicio bebidaPreparadaValidacionServicio,
            IImagenServicio imagenServicio,
            INormalizadorNombreServicio normalizadorNombreServicio,
            ILogger<CrearBebidaPreparadaCasoDeUso> logger)
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
            _logger.LogInformation("Iniciando creación de la bebida preparada '{Nombre}' para el restaurante {RestauranteId}.", bebidaPreparada.Nombre, restauranteId);

            _bebidaPreparadaValidacionServicio.ValidarDatosBasicos(bebidaPreparada);

            bebidaPreparada.Nombre = _normalizadorNombreServicio.Normalizar(bebidaPreparada.Nombre);

            bool existeNombre = await _bebidaPreparadaRepositorio.ExisteBebidaPreparadaConNombreAsync(restauranteId, bebidaPreparada.Nombre);
            if (existeNombre)
            {
                _logger.LogWarning("Rechazo al crear bebida preparada: Ya existe una con el nombre '{Nombre}' en el restaurante {RestauranteId}.", bebidaPreparada.Nombre, restauranteId);
                throw new ArgumentException($"Ya existe una bebida preparada con el nombre '{bebidaPreparada.Nombre}' en el restaurante.");
            }

            await _insumoValidacionServicio.ValidarInsumosDeRecetaBebidaAsync(restauranteId, bebidaPreparada.Insumos);

            if (stream != null && !string.IsNullOrEmpty(nombreImagen))
            {
                bebidaPreparada.UrlImagen = await _imagenServicio.SubirImagenAsync(stream, nombreImagen, carpetaCloudinary);
            }

            bebidaPreparada.RestauranteId = restauranteId;

            BebidaPreparada bebidaCreada = await _bebidaPreparadaRepositorio.CrearAsync(bebidaPreparada);

            _logger.LogInformation("Bebida preparada '{Nombre}' creada exitosamente con ID {Id} en el restaurante {RestauranteId}.", bebidaCreada.Nombre, bebidaCreada.Id, restauranteId);

            return bebidaCreada;
        }
    }
}
