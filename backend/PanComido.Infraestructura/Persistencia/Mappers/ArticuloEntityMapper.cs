using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class ArticuloEntityMapper
    {
        private readonly PlatoEntityMapper _platoMapper;
        private readonly InsumoEntityMapper _insumoMapper;

        public ArticuloEntityMapper(PlatoEntityMapper platoMapper, InsumoEntityMapper insumoMapper)
        {
            _platoMapper = platoMapper;
            _insumoMapper = insumoMapper;
        }
        public DOM.Articulo paraDominio(EF.Articulo efArticulo)
        {
            if (efArticulo == null) return null;

            DOM.Articulo articuloBase;

            if (efArticulo.Plato != null)
            {
                articuloBase = _platoMapper.CompletarMapeoDominio(efArticulo);
            }
            else if (efArticulo.Insumo != null)
            {
                articuloBase = _insumoMapper.CompletarMapeoDominio(efArticulo);
            }
            else
            {
                throw new InvalidOperationException($"Articulo ID {efArticulo.Id} no tiene subclase válida.");
            }
            // 2. Mapeamos las propiedades del padre 
            articuloBase.Id = efArticulo.Id;
            articuloBase.RestauranteId = efArticulo.RestauranteId;
            articuloBase.CartaId = efArticulo.CartaId;
            articuloBase.Nombre = efArticulo.Nombre;
            articuloBase.Descripcion = efArticulo.Descripcion;
            articuloBase.PrecioVentaFinal = efArticulo.PrecioVentaFinal;
            articuloBase.PrecioGanancia = efArticulo.PrecioGanancia;
            articuloBase.PrecioPromocional = efArticulo.PrecioPromocional;
            articuloBase.UrlImagen = efArticulo.UrlImagen;
            articuloBase.EsVisibleEnCarta = efArticulo.ConfiguracionArticulos?.Any(c => c.Id == 2) ?? false;
            articuloBase.EsPrecioManual = efArticulo.EsPrecioManual;
            return articuloBase;
        }

        public EF.Articulo paraEntidad(DOM.Articulo articuloDominio)
        {
            if (articuloDominio == null) return null;
            var efArticulo = new EF.Articulo
            {
                Id = articuloDominio.Id,
                RestauranteId = articuloDominio.RestauranteId,
                CartaId = articuloDominio.CartaId > 0 ? articuloDominio.CartaId : null,
                Nombre = articuloDominio.Nombre,
                Descripcion = articuloDominio.Descripcion,
                PrecioVentaFinal = articuloDominio.PrecioVentaFinal,
                PrecioGanancia = articuloDominio.PrecioGanancia,
                PrecioPromocional = articuloDominio.PrecioPromocional,
                UrlImagen = articuloDominio.UrlImagen,
                EsPrecioManual = articuloDominio.EsPrecioManual
            };
            if (articuloDominio is DOM.Insumo insumoDominio)
            {
                efArticulo.Insumo = _insumoMapper.CompletarMapeoAEntidad(insumoDominio);
            }
            else if (articuloDominio is DOM.Plato platoDominio)
            {
                efArticulo.Plato = _platoMapper.CompletarMapeoAEntidad(platoDominio);
            }
            else
            {
                throw new InvalidOperationException("El tipo de artículo de dominio no está soportado para guardar.");
            }
            return efArticulo;
        }
    }
}
