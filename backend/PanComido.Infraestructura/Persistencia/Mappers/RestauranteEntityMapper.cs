using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class RestauranteEntityMapper
    {
        public DOM.Restaurante paraDominio(EF.Restaurante efRestaurante)
        {
            return new DOM.Restaurante
            {
                Id = efRestaurante.Id,
                Nombre = efRestaurante.Nombre,
                Imagen = efRestaurante.Imagen,
                ColorPrincipal = efRestaurante.ColorPrincipal,
                ColorSecundario = efRestaurante.ColorSecundario,
                TextoPrincipal = efRestaurante.TextoPrincipal,
                TextoSecundario = efRestaurante.TextoSecundario,
                DireccionId = efRestaurante.DireccionId,
                Ubicacion = new DOM.Ubicacion
                {
                    Id = efRestaurante.Direccion.Id,
                    Direccion = efRestaurante.Direccion.Direccion,
                    Ciudad = efRestaurante.Direccion.Ciudad,
                    CodigoPostal = efRestaurante.Direccion.CodigoPostal
                }
            };
        }

        public void paraActualizarEntidad(EF.Restaurante efRestoDatosExistentes, DOM.Restaurante restauranteDatosNuevos)
        {

            efRestoDatosExistentes.Nombre = restauranteDatosNuevos.Nombre;
            efRestoDatosExistentes.Imagen = restauranteDatosNuevos.Imagen;
            efRestoDatosExistentes.ColorPrincipal = restauranteDatosNuevos.ColorPrincipal;
            efRestoDatosExistentes.ColorSecundario = restauranteDatosNuevos.ColorSecundario;
            efRestoDatosExistentes.TextoPrincipal = restauranteDatosNuevos.TextoPrincipal;
            efRestoDatosExistentes.TextoSecundario = restauranteDatosNuevos.TextoSecundario;

        }
    }
}
