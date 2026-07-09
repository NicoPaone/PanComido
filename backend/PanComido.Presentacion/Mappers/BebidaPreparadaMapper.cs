using PanComido.Presentacion.DTOs.BebidaPreparada;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class BebidaPreparadaMapper
    {
        public DOM.BebidaPreparada aDominio(CrearBebidaPreparadaRequestDto crearBebidaPreparadaRequestDto)
        {
            if (crearBebidaPreparadaRequestDto == null) return null;

            return new DOM.BebidaPreparada
            {
                Nombre = crearBebidaPreparadaRequestDto.Nombre,
                Descripcion = crearBebidaPreparadaRequestDto.Descripcion,
                PrecioVentaFinal = crearBebidaPreparadaRequestDto.PrecioVentaFinal,
                EsPrecioManual = crearBebidaPreparadaRequestDto.EsPrecioManual,
                EsVisibleEnCarta = crearBebidaPreparadaRequestDto.EsVisibleEnCarta,
                Insumos = crearBebidaPreparadaRequestDto.Insumos != null
                    ? crearBebidaPreparadaRequestDto.Insumos.Select(i => new DOM.BebidaPreparadaInsumo
                    {
                        InsumoId = i.InsumoId,
                        Cantidad = i.Cantidad
                    }).ToList()
                    : new List<DOM.BebidaPreparadaInsumo>()
            };
        }

        public DOM.BebidaPreparada ModificarADominio(int id, ModificarBebidaPreparadaRequestDto modificarBebidaPreparadaRequestDto)
        {
            if (modificarBebidaPreparadaRequestDto == null) return null;

            return new DOM.BebidaPreparada
            {
                Id = id,
                Nombre = modificarBebidaPreparadaRequestDto.Nombre,
                Descripcion = modificarBebidaPreparadaRequestDto.Descripcion,
                PrecioVentaFinal = modificarBebidaPreparadaRequestDto.PrecioVentaFinal,
                EsPrecioManual = modificarBebidaPreparadaRequestDto.EsPrecioManual,
                EsVisibleEnCarta = modificarBebidaPreparadaRequestDto.EsVisibleEnCarta,
                Insumos = modificarBebidaPreparadaRequestDto.Insumos != null
                    ? modificarBebidaPreparadaRequestDto.Insumos.Select(i => new DOM.BebidaPreparadaInsumo
                    {
                        InsumoId = i.InsumoId,
                        Cantidad = i.Cantidad
                    }).ToList()
                    : new List<DOM.BebidaPreparadaInsumo>()
            };
        }

        public DetalleBebidaPreparadaResponseDto aDto(DOM.BebidaPreparada bebidaPreparadaDominio)
        {
            if (bebidaPreparadaDominio == null) return null;

            return new DetalleBebidaPreparadaResponseDto
            {
                Id = bebidaPreparadaDominio.Id,
                Nombre = bebidaPreparadaDominio.Nombre,
                Descripcion = bebidaPreparadaDominio.Descripcion,
                PrecioVentaFinal = bebidaPreparadaDominio.PrecioVentaFinal ?? 0,
                UrlImagen = bebidaPreparadaDominio.UrlImagen,
                EsPrecioManual = bebidaPreparadaDominio.EsPrecioManual,
                EsVisibleEnCarta = bebidaPreparadaDominio.EsVisibleEnCarta,
                Categoria = bebidaPreparadaDominio.Categoria,
                Insumos = bebidaPreparadaDominio.Insumos?.Select(i => new InsumoRecetaResponseDto
                {
                    InsumoId = i.InsumoId,
                    Cantidad = i.Cantidad,
                    Nombre = i.Insumo?.Nombre
                }).ToList() ?? new List<InsumoRecetaResponseDto>()
            };
        }
    }
}
