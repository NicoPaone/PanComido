using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.Carta;

namespace PanComido.Presentacion.Mappers
{
    public class ArticuloCartaMapper
    {

        // Convierte un solo artículo de Dominio al DTO de Angular
        public ArticuloCartaResponseDto aDto(Articulo articulo)
        {
            return new ArticuloCartaResponseDto
            {
                ArticuloId = articulo.Id,
                Nombre = articulo.Nombre ?? "",
                UrlImagen = articulo.UrlImagen,

                PrecioVentaFinal = articulo.PrecioVentaFinal ?? 0,

                // Leemos si está visible o no
                VisibleEnCarta = articulo.EsVisibleEnCarta,

                // Magia para que Angular sepa si pone el iconito de plato o bebida
                TipoArticulo = articulo is Plato ? "Plato" : "Bebida",

                // ¡Acá inyectamos el resultado matemático que calculó el Caso de Uso!
                Costo = articulo.CostoCalculado,

                // Inyectamos la categoria para el filtrado en frontend
                Categoria = articulo is Plato p ? (p.Categoria ?? "Sin categoria") : (articulo is Insumo i ? (i.Categoria ?? "Sin categoria") : "Sin categoria"),
                TiempoPreparacionBase = articulo is Plato plato ? plato.TiempoPreparacionBase : default
            };
        }

        // Convierte la lista entera usando el método de arriba
        public List<ArticuloCartaResponseDto> aListaDto(List<Articulo> articulos)
        {

            return articulos.Select(aDto).ToList();
        }
    }



}

