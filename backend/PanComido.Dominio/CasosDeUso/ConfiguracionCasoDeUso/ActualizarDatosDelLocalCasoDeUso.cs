using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso
{
    public class ActualizarDatosDelLocalCasoDeUso
    {
        private readonly IRestauranteRepositorio _restauranteRepositorio;
        private readonly IImagenServicio _servicioImagen;
        private readonly ILogger<ActualizarDatosDelLocalCasoDeUso> _logger;

        public ActualizarDatosDelLocalCasoDeUso(IRestauranteRepositorio restauranteRepositorio, IImagenServicio servicioImagen, ILogger<ActualizarDatosDelLocalCasoDeUso> logger)
        {
            _restauranteRepositorio = restauranteRepositorio;
            _servicioImagen = servicioImagen;
            _logger = logger;
        }

        public async Task<Restaurante> EjecutarAsync(
           int restauranteId,
           Restaurante restauranteDatos,
           string carpetaCloudinary,
           Stream streamImagen,
           string nombreImagen)
        {
            var datosExistentes = await _restauranteRepositorio.ObtenerDatosDelLocalAsync(restauranteId);
            if (datosExistentes == null) throw new KeyNotFoundException("Restaurante no encontrado");

            string? urlImagen = null;

            if (streamImagen != null && !string.IsNullOrEmpty(nombreImagen))
            {
                urlImagen = await _servicioImagen.SubirImagenAsync(streamImagen, nombreImagen, carpetaCloudinary);
                _logger.LogInformation("Imagen subida a Cloudinary. RestauranteId: {RestauranteId}, NombreImagen: {NombreImagen}", restauranteId, nombreImagen);
                restauranteDatos.Imagen = urlImagen;
            }
            else
            {
                //evitar que se setee a null la img al actualizar otro dato
                restauranteDatos.Imagen = datosExistentes.Imagen;
            }

            await _restauranteRepositorio.ActualizarDatosDelLocalAsync(restauranteId, restauranteDatos);
            _logger.LogInformation("Datos del local actualizados. RestauranteId: {RestauranteId}", restauranteId);

            var actualizado = await _restauranteRepositorio.ObtenerDatosDelLocalAsync(restauranteId);
            return actualizado!;
        }
    }
}