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

        public DOM.Plato ModificarADominio(int id, ModificarPlatoDto platoDto)
        {
            if (platoDto == null)
                return null;

            return new DOM.Plato
            {
                Id = id,
                Nombre = platoDto.Nombre,
                Descripcion = platoDto.Descripcion,
                PrecioVentaFinal = platoDto.PrecioVentaFinal,
                TiempoPreparacionBase = platoDto.TiempoPreparacionBase,
                TipoPlatoId = platoDto.TipoPlatoId,
                CategoriaPlatoId = platoDto.CategoriaPlatoId,
                UrlImagen = platoDto.UrlImagen,
                EsVisibleEnCarta = platoDto.EsVisibleEnCarta,

                Restricciones = platoDto.RestriccionesIds != null
                    ? platoDto.RestriccionesIds.Select(rId => new DOM.Restriccion { Id = rId }).ToList()
                    : new List<DOM.Restriccion>(),

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

        public DetallePlatoResponseDto aDto(DOM.Plato platoDominio)
        {
            if (platoDominio == null) return null;

            return new DetallePlatoResponseDto
            {
                Id = platoDominio.Id,
                Nombre = platoDominio.Nombre,
                Descripcion = platoDominio.Descripcion,
                PrecioVentaFinal = platoDominio.PrecioVentaFinal ?? 0,
                TiempoPreparacionBase = platoDominio.TiempoPreparacionBase,
                TipoPlatoId = platoDominio.TipoPlatoId,
                CategoriaPlatoId = platoDominio.CategoriaPlatoId,
                UrlImagen = platoDominio.UrlImagen,
                EsVisibleEnCarta = platoDominio.EsVisibleEnCarta,
                RestriccionesIds = platoDominio.Restricciones?.Select(r => r.Id).ToList() ?? new List<int>(),
                Ingredientes = platoDominio.Ingredientes?.Select(i => new IngredienteRecetaDto
                {
                    InsumoId = i.InsumoId,
                    Cantidad = i.Cantidad,
                    Opcional = i.Opcional
                    ,
                    Nombre = i.Insumo?.Nombre
                }).ToList() ?? new List<IngredienteRecetaDto>()
            };
        }
    }
}
