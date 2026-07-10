using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.Articulos;

namespace PanComido.Presentacion.Mappers
{
    public class CartaComensalMapper
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
                dto.TiempoPreparacionEstimado = plato.TiempoPreparacionEstimado;
                
                dto.CategoriaPlato = plato.Categoria;
                dto.TipoPlato = plato.TipoPlato;
                dto.Restricciones = plato.Restricciones?.Select(r => r.Descripcion).ToList() ?? new List<string>();
            }
            else if (articuloDominio is Insumo bebida)
            {
                dto.EsPlato = false;
                dto.CategoriaBebida = bebida.Categoria;
            }
            else if (articuloDominio is BebidaPreparada bebidaPreparada)
            {
                dto.EsPlato = false;
                dto.CategoriaBebida = bebidaPreparada.Categoria;
            }

                return dto;
        }

        public List<ArticuloCartaDto> ParaDtoList(List<Articulo> articulosDominio)
        {
            return articulosDominio.Select(a => ParaDto(a)).ToList();
        }
    }
}
