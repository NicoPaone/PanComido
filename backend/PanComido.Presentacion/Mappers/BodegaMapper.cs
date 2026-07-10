using PanComido.Presentacion.DTOs.Bodegas;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class BodegaMapper
    {
        private readonly InsumoMapper _insumoMapper;
        public BodegaMapper(InsumoMapper insumoMapper)
        {
            _insumoMapper = insumoMapper;
        }

        public BodegaResponseDto bodegaADto(DOM.Bodega bodega)
        {
            return new BodegaResponseDto
            {
                Id = bodega.Id,
                Nombre = bodega.Nombre,
                TipoBodega = bodega.TipoBodega,
                TipoBodegaId = bodega.TipoBodegaId
            };
        }

        public List<BodegaResponseDto> bodegasAListaDto(List<DOM.Bodega> bodegas)
        {
            return bodegas
                .Select(b => bodegaADto(b))
                .ToList();
        }

        public BodegaConInsumosResponseDto bodegaConInsumosADto(DOM.Bodega bodega)
        {
            return new BodegaConInsumosResponseDto
            {
                Id = bodega.Id,
                Nombre = bodega.Nombre,
                TipoBodega = bodega.TipoBodega,
                TipoBodegaId = bodega.TipoBodegaId,
                Insumos = _insumoMapper.aListaDto(bodega.Insumos)
            };
        }

        public List<BodegaConInsumosResponseDto> bodegasConInsumosAListaDto(List<DOM.Bodega> bodegas)
        {
            return bodegas
                .Select(b => bodegaConInsumosADto(b))
                .ToList();
        }
        public DOM.Bodega aDominio(GuardarBodegaRequestDto dto, int? id = null)
        {
            if (dto == null) return null;
            return new DOM.Bodega
            {
                Id = id ?? 0,
                Nombre = dto.Nombre,
                TipoBodegaId = dto.TipoBodegaId
            };
        }
    }
}
