using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.Carta;

namespace PanComido.Presentacion.Mappers
{
    public class ArticuloCartaMapper
    {
        public ArticuloCartaResponseDto aDto(Articulo articulo)
        {
            Plato plato = articulo as Plato;
            Insumo insumo = articulo as Insumo;

            bool esPlato = plato != null;
            bool esInsumo = insumo != null;

            return new ArticuloCartaResponseDto
            {
                ArticuloId = articulo.Id,
                Nombre = articulo.Nombre ?? "",
                UrlImagen = articulo.UrlImagen,

                PrecioVentaFinal = articulo.PrecioVentaFinal ?? 0,
                VisibleEnCarta = articulo.EsVisibleEnCarta,

                TipoArticulo = esPlato ? "Plato" : "Bebida",
                Costo = articulo.CostoCalculado,

                //luketo
                Categoria = esPlato
                    ? (plato.Categoria ?? "Sin categoria")
                    : (esInsumo
                        ? (insumo.Categoria ?? "Sin categoria")
                        : "Sin categoria"),

                TiempoPreparacionBase = esPlato ? plato.TiempoPreparacionBase : default,
                TiempoPreparacionEstimado = esPlato ? plato.TiempoPreparacionEstimado : default

                    //maga
                // // Inyectamos la categoria para el filtrado en frontend
               // Categoria = articulo is Plato p ? (p.Categoria ?? "Sin categoria") : (articulo is Insumo i ? (i.Categoria ?? "Sin categoria") : "Sin categoria"),
               // TiempoPreparacionBase = articulo is Plato plato ? plato.TiempoPreparacionBase : default,

               // Restricciones = articulo is Plato platoConRestricciones ? platoConRestricciones.Restricciones.Select(r => r.Descripcion).ToList(): new List<string>()

            };
        }

        public List<ArticuloCartaResponseDto> aListaDto(List<Articulo> articulos)
        {

            return articulos.Select(aDto).ToList();
        }
    }



}

