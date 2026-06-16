using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanComido.Dominio.Constantes;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.CasosDeUso.PlatoCasosDeUso
{
    public  class CrearPlatoCasoDeUso
    {
        private readonly IPlatoRepositorio _platoRepositorio;

      private readonly IImagenServicio _servicioImagen;

      public CrearPlatoCasoDeUso(IPlatoRepositorio platoRepositorio, IImagenServicio servicio)
        {
            _platoRepositorio = platoRepositorio;
         _servicioImagen = servicio;
        }   

        public async Task EjecutarAsync ( int restauranteID, Plato plato, string carpetaCloudinary, Stream stream, string nombreImagen)
        {

            if ( string.IsNullOrWhiteSpace(plato.Nombre) )
            {
                throw new ArgumentException("El nombre del plato no puede estar vacío.");
            }
            if (plato.PrecioVentaFinal <= 0)
            {
                throw new ArgumentException("El precio de venta final debe ser mayor que cero.");
            }

            if ( plato.Ingredientes == null || !plato.Ingredientes.Any() )
            {
                throw new ArgumentException("El plato debe tener al menos un ingrediente.");
            }
         string?  urlImagen = null;

         if (stream != null && !string.IsNullOrEmpty(nombreImagen))
         {
            urlImagen = await _servicioImagen
               .SubirImagenAsync(stream, nombreImagen, carpetaCloudinary);

            plato.UrlImagen= urlImagen;

         }
         plato.RestauranteId = restauranteID;   

            await _platoRepositorio.CrearAsync(plato);
        }
    }
}
