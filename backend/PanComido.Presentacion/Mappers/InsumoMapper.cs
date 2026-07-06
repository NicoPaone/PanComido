using PanComido.Presentacion.DTOs.Insumos;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class InsumoMapper
    {
        public InsumoResponseDto aDto(DOM.Insumo insumo)
        {
            return new InsumoResponseDto
            {
                Id = insumo.Id,
                Nombre = insumo.Nombre,
                StockActual = insumo.StockActual,
                UnidadMedida = insumo.UnidadMedida,
                Vencimiento = insumo.Vencimiento?.ToString("dd/MM/yyyy"),
                StockMinimo = insumo.StockMinimo,
                StockRecomendado = insumo.StockRecomendado,
                PrecioVentaFinal = insumo.PrecioVentaFinal ?? 0,
                EstadoStock = insumo.EstadoStock?.ToString(),
                Tipo = insumo.Tipo.ToString(),
                Categoria = insumo.Categoria,
                EsPrecioManual = insumo.EsPrecioManual
            };
        }
        public List<InsumoResponseDto> aListaDto(
            List<DOM.Insumo> insumos)
        {
            return insumos
                .Select(i => aDto(i))
                .ToList();
        }

        public DOM.Insumo aDominio(CrearInsumoRequestDto request)
        {
            if (request == null) return null;

            return new DOM.Insumo
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                PrecioVentaFinal = request.PrecioVentaFinal,
                EsPrecioManual = request.EsPrecioManual,
                StockMinimo = request.StockMinimo,
                StockRecomendado = request.StockRecomendado,
                CategoriaId = request.CategoriaId,
                UnidadDeMedidaId = request.UnidadDeMedidaId
            };
        }

        public DOM.Insumo ModificarADominio(int id, ModificarInsumoRequestDto insumoRequest)
        {
            if (insumoRequest == null) return null;
            return new DOM.Insumo
            {
                Id = id,
                Nombre = insumoRequest.Nombre,
                Descripcion = insumoRequest.Descripcion,
                PrecioVentaFinal = insumoRequest.PrecioVentaFinal,
                EsPrecioManual = insumoRequest.EsPrecioManual,
                StockMinimo = insumoRequest.StockMinimo,
                StockRecomendado = insumoRequest.StockRecomendado,
                CategoriaId = insumoRequest.CategoriaId,
                UnidadDeMedidaId = insumoRequest.UnidadDeMedidaId
            };
        }

        public DetalleInsumoResponseDto aDetalleDto(DOM.Insumo insumo)
        {
            return new DetalleInsumoResponseDto
            {
                Id = insumo.Id,
                Nombre = insumo.Nombre,
                Descripcion = insumo.Descripcion,
                PrecioVentaFinal = insumo.PrecioVentaFinal,
                EsPrecioManual = insumo.EsPrecioManual,
                StockMinimo = insumo.StockMinimo,
                StockRecomendado = insumo.StockRecomendado,
                CategoriaId = insumo.CategoriaId,
                UnidadDeMedidaId = insumo.UnidadDeMedidaId,
                UrlImagen = insumo.UrlImagen,
                Tipo = insumo.Tipo.ToString()
            };
        }
    }
}
