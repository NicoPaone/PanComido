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

                Destacado = esPlato ? plato.Destacado : false,
                TipoArticulo = esPlato ? "Plato" : "Bebida",
                Costo = articulo.CostoCalculado,
                Categoria = esPlato
                    ? (plato.Categoria ?? "Sin categoria")
                    : (esInsumo
                        ? (insumo.Categoria ?? "Sin categoria")
                        : "Sin categoria"),
                CategoriaPlatoId = esPlato ? plato.CategoriaPlatoId : null,
                TiempoPreparacionBase = esPlato ? plato.TiempoPreparacionBase : default,
                TiempoPreparacionEstimado = esPlato ? plato.TiempoPreparacionEstimado : default,
                EsPrecioManual = articulo.EsPrecioManual,

                Restricciones = esPlato 
                    ? plato.Restricciones.Select(r => r.Descripcion).ToList() 
                    : new List<string>()
            };
        }

        public List<ArticuloCartaResponseDto> aListaDto(List<Articulo> articulos)
        {

            return articulos.Select(aDto).ToList();
        }
    }



}

