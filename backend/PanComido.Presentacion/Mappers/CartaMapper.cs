using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.Articulos;

namespace PanComido.Presentacion.Mappers
{
    public class CartaMapper
    {
        public ArticuloCartaDto ParaDto(Articulo articuloDominio)
        {
            var dto = new ArticuloCartaDto
            {
                Id = articuloDominio.Id,
                Nombre = articuloDominio.Nombre,
                Descripcion = articuloDominio.Descripcion,
                Precio = articuloDominio.PrecioVentaFinal ?? 0,
                UrlImagen = articuloDominio.UrlImagen
            };

            if (articuloDominio is Plato plato)
            {
                dto.EsPlato = true;
                dto.EsDestacado = plato.Destacado;
                dto.TiempoPreparacionBase = plato.TiempoPreparacionBase;

                dto.CategoriaPlato = plato.Categoria;
                dto.TipoPlato = plato.TipoPlato;
                dto.Restricciones = plato.Restricciones ?? new List<string>();
            }
            else if (articuloDominio is Insumo bebida)
            {
                dto.EsPlato = false;
                dto.CategoriaBebida = bebida.Categoria;
            }

            return dto;
        }

        public List<ArticuloCartaDto> ParaDtoList(List<Articulo> articulosDominio)
        {
            return articulosDominio.Select(a => ParaDto(a)).ToList();
        }
    }
}
