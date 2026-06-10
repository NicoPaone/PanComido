using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.Carta;

namespace PanComido.Presentacion.Mappers
{
    public class ArticuloCartaMapper
    {
        public ArticuloCartaResponseDto aDto(Articulo articulo)
        {
            return new ArticuloCartaResponseDto
            {
                ArticuloId = articulo.Id,
                Nombre = articulo.Nombre ?? "",
                UrlImagen = articulo.UrlImagen,
                PrecioVentaFinal = articulo.PrecioVentaFinal ?? 0,

                VisibleEnCarta = articulo.EsVisibleEnCarta,

                TipoArticulo = articulo is Plato ? "Plato" : "Bebida",

                Costo = articulo.CostoCalculado,

                Categoria = articulo is Plato p ? (p.Categoria ?? "Sin categoria") : (articulo is Insumo i ? (i.Categoria ?? "Sin categoria") : "Sin categoria"),
                TiempoPreparacionBase = articulo is Plato plato ? plato.TiempoPreparacionBase : default
            };
        }

        public List<ArticuloCartaResponseDto> aListaDto(List<Articulo> articulos)
        {
         
            return articulos.Select(aDto).ToList();
        }
    }



}

