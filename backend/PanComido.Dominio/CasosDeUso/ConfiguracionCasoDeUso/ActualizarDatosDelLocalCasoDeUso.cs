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

        public ActualizarDatosDelLocalCasoDeUso(IRestauranteRepositorio restauranteRepositorio, IImagenServicio servicioImagen) 
        {
            _restauranteRepositorio = restauranteRepositorio;
          _servicioImagen = servicioImagen;
        }

        public async Task<Restaurante> EjecutarAsync(
           int restauranteId, 
           Restaurante restauranteDatos,
           string carpetaCloudinary,
           Stream streamImagen,
           string nombreImagen) 
        {
         string? urlImagen = null;

         if(streamImagen != null && !string.IsNullOrEmpty(nombreImagen))
         {
            urlImagen = await _servicioImagen
               .SubirImagenAsync(streamImagen, nombreImagen, carpetaCloudinary);

            restauranteDatos.Imagen = urlImagen;

         }
            await _restauranteRepositorio.ActualizarDatosDelLocalAsync(restauranteId, restauranteDatos);

            return await _restauranteRepositorio.ObtenerDatosDelLocalAsync(restauranteId);
        }
    }
}
