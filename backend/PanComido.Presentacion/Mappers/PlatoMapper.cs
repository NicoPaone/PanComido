using PanComido.Presentacion.DTOs.Plato;
using DOM = PanComido.Dominio.Entidades;


namespace PanComido.Presentacion.Mappers
{
    public class PlatoMapper
    {
        public DOM.Plato aDominio (CrearPlatoDto platoDto)
        {
            if ( platoDto == null)
                return null;

            return new DOM.Plato
            {
                Nombre = platoDto.Nombre,
                Descripcion = platoDto.Descripcion,
                PrecioVentaFinal = platoDto.PrecioVentaFinal,
                TiempoPreparacionBase = platoDto.TiempoPreparacionBase,
                TipoPlatoId = platoDto.TipoPlatoId,
                CategoriaPlatoId = platoDto.CategoriaPlatoId,
                UrlImagen = platoDto.UrlImagen,


                // Mapeamos la lista de números (IDs) a una lista de objetos Restriccion
                // Solo le seteamos el Id. El repositorio se va a encargar de buscar el resto en la BD.
                Restricciones = platoDto.RestriccionesIds != null
                    ? platoDto.RestriccionesIds.Select(id => new DOM.Restriccion { Id = id }).ToList()
                    : new List<DOM.Restriccion>(),

                // Traducimos los paquetitos de ingredientes de Angular a la entidad intermedia de nuestro Dominio
                Ingredientes = platoDto.Ingredientes != null
                    ? platoDto.Ingredientes.Select(i => new DOM.PlatoIngrediente
                    {
                        InsumoId = i.InsumoId,
                        Cantidad = i.Cantidad,
                        Opcional = i.Opcional
                    }).ToList()
                    : new List<DOM.PlatoIngrediente>()

            };
        }




    }
}
