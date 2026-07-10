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
        private readonly IInsumoValidacionServicio _insumoValidacionServicio;

      private readonly IImagenServicio _servicioImagen;
      private readonly INormalizadorNombreServicio _normalizadorNombreServicio;

      public CrearPlatoCasoDeUso(IPlatoRepositorio platoRepositorio, IInsumoValidacionServicio insumoValidacionServicio, IImagenServicio servicio, INormalizadorNombreServicio normalizadorNombreServicio)
        {
            _platoRepositorio = platoRepositorio;
            _insumoValidacionServicio = insumoValidacionServicio;
         _servicioImagen = servicio;
         _normalizadorNombreServicio = normalizadorNombreServicio;
        }

        public async Task EjecutarAsync ( int restauranteID, Plato plato, string carpetaCloudinary, Stream stream, string nombreImagen)
        {

            if ( string.IsNullOrWhiteSpace(plato.Nombre) )
            {
                throw new ArgumentException("El nombre del plato no puede estar vacío.");
            }

            plato.Nombre = _normalizadorNombreServicio.Normalizar(plato.Nombre);

            if (plato.PrecioVentaFinal <= 0)
            {
                throw new ArgumentException("El precio de venta final debe ser mayor que cero.");
            }

            if ( plato.Ingredientes == null || !plato.Ingredientes.Any() )
            {
                throw new ArgumentException("El plato debe tener al menos un ingrediente.");
            }

            if (plato.Ingredientes.Any(i => i.Cantidad <= 0))
            {
                throw new ArgumentException("La cantidad de cada ingrediente debe ser mayor que cero.");
            }

            if (plato.Ingredientes.Select(i => i.InsumoId).Distinct().Count() != plato.Ingredientes.Count)
            {
                throw new ArgumentException("No se puede repetir el mismo insumo en los ingredientes del plato.");
            }

            var insumoIds = plato.Ingredientes.Select(i => i.InsumoId).ToList();
            await _insumoValidacionServicio.ValidarInsumosActivosAsync(insumoIds, restauranteID);

            bool existePlato = await _platoRepositorio.ExistePlatoConNombreAsync(restauranteID, plato.Nombre);
            if (existePlato)
            {
                throw new ArgumentException($"Ya existe un plato con el nombre '{plato.Nombre}' en el restaurante.");
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
