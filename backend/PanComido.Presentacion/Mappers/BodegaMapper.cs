using PanComido.Presentacion.DTOs;
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

        public BodegaResponseDto aDto(DOM.Bodega bodega)
        {
            return new BodegaResponseDto
            {
                Id = bodega.Id,
                Nombre = bodega.Nombre,

                Insumos = _insumoMapper.aListaDto(bodega.Insumos)
            };
        }

        public List<BodegaResponseDto> aListaDto(List<DOM.Bodega> bodegas)
        {
            return bodegas
                .Select(b => aDto(b))
                .ToList();
        }
    }
}
