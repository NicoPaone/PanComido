using PanComido.Presentacion.DTOs.Restaurante;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class RestauranteMapper
    {
        public RestauranteResponseDto aDto(DOM.Restaurante restaurante)
        {
            return new RestauranteResponseDto
            {
                Id = restaurante.Id,
                Nombre = restaurante.Nombre,
                Imagen = restaurante.Imagen,
                ColorPrincipal = restaurante.ColorPrincipal,
                ColorSecundario = restaurante.ColorSecundario,
                Direccion = restaurante.Ubicacion?.Direccion,
                FamiliaTipograficaId = restaurante.FamiliaTipograficaId,
                FamiliaCategoria = restaurante.FamiliaTipografica?.Categoria,
                TipografiaTitulo = restaurante.FamiliaTipografica?.TipografiaTitulo,
                TipografiaCuerpo = restaurante.FamiliaTipografica?.TipografiaCuerpo,
                LinkResenaGoogleMaps = restaurante.LinkResenaGoogleMaps
            };
        }

        public DOM.Restaurante aDominio(RestauranteRequestDto restauranteRequest)
        {
            return new DOM.Restaurante
            {
                Nombre = restauranteRequest.Nombre,
                ColorPrincipal = restauranteRequest.ColorPrincipal,
                ColorSecundario = restauranteRequest.ColorSecundario,
                FamiliaTipograficaId = restauranteRequest.FamiliaTipograficaId,
                LinkResenaGoogleMaps = restauranteRequest.LinkResenaGoogleMaps
            };
        }
    }
}