using PanComido.Presentacion.DTOs.MetodoDePago;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class MetodoDePagoMapper
    {
        public MetodoDePagoResponseDto aDto(DOM.MetodoDePago metodoDePago)
        {
            return new MetodoDePagoResponseDto
            {
                Id = metodoDePago.Id,
                Descripcion = metodoDePago.Descripcion,
                Habilitado = metodoDePago.Habilitado
            };
        }

        public List<MetodoDePagoResponseDto> aListaDto(List<DOM.MetodoDePago> metodosDePago)
        {
            return metodosDePago.Select(mp => aDto(mp)).ToList();
        }

        public DOM.MetodoDePago aDominio(MetodoDePagoRequestDto metodoDePagoRequest)
        {
            return new DOM.MetodoDePago
            {
                Id = metodoDePagoRequest.Id,
                Habilitado = metodoDePagoRequest.Habilitado
            };
        }

        public List<DOM.MetodoDePago> aListaDominio(List<MetodoDePagoRequestDto> metodosDePagoRequest)
        { 
            return metodosDePagoRequest.Select(mp => aDominio(mp)).ToList();
        }
    }
}
