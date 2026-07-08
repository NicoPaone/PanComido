using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.Encuesta;

namespace PanComido.Presentacion.Mappers
{
    public class EncuestaMapper
    {
        public EncuestaSatisfaccion RequestDtoADominio(EncuestaRequestDto request)
        {
            return new EncuestaSatisfaccion
            {
                ComandaId = request.ComandaId,
                PuntuacionLugar = request.PuntuacionLugar,
                PuntuacionComida = request.PuntuacionComida,
                PuntuacionMozo = request.PuntuacionMozo
            };
        }
        public EncuestaResponseDto GoogleLinkAResponseDto(string? googleLink)
        {
            return new EncuestaResponseDto
            {
                Success = true, 
                LinkResenaGoogleMaps = googleLink
            };
        }
    }
}
