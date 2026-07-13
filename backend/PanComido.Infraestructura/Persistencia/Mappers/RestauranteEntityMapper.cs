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
                DireccionId = efRestaurante.DireccionId,
                FamiliaTipograficaId = efRestaurante.FamiliaTipograficaId,
                Ubicacion = new DOM.Ubicacion
                {
                    Id = efRestaurante.Direccion.Id,
                    Direccion = efRestaurante.Direccion.Direccion,
                    Ciudad = efRestaurante.Direccion.Ciudad,
                    CodigoPostal = efRestaurante.Direccion.CodigoPostal
                },
                FamiliaTipografica = efRestaurante.FamiliaTipografica == null ? null : new DOM.FamiliaTipografica
                {
                    Id = efRestaurante.FamiliaTipografica.Id,
                    Categoria = efRestaurante.FamiliaTipografica.Categoria,
                    TipografiaTitulo = efRestaurante.FamiliaTipografica.TipografiaTitulo,
                    TipografiaCuerpo = efRestaurante.FamiliaTipografica.TipografiaCuerpo
                },
                LinkResenaGoogleMaps = efRestaurante.LinkResenaGoogleMaps
            };
        }

        public void paraActualizarEntidad(EF.Restaurante efRestoDatosExistentes, DOM.Restaurante restauranteDatosNuevos)
        {
            efRestoDatosExistentes.Nombre = restauranteDatosNuevos.Nombre;
            efRestoDatosExistentes.Imagen = restauranteDatosNuevos.Imagen;
            efRestoDatosExistentes.ColorPrincipal = restauranteDatosNuevos.ColorPrincipal;
            efRestoDatosExistentes.ColorSecundario = restauranteDatosNuevos.ColorSecundario;
            efRestoDatosExistentes.FamiliaTipograficaId = restauranteDatosNuevos.FamiliaTipograficaId;
            efRestoDatosExistentes.LinkResenaGoogleMaps = restauranteDatosNuevos.LinkResenaGoogleMaps;
            efRestoDatosExistentes.Direccion.Direccion = restauranteDatosNuevos.Ubicacion?.Direccion;
        }
    }
}