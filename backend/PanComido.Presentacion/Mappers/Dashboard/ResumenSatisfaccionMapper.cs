using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.Dashboard;

namespace PanComido.Presentacion.Mappers.Dashboard
{
    public static class ResumenSatisfaccionMapper
    {
        public static SatisfaccionResponseDto AResponseDto(ResumenSatisfaccion resumen)
        {
            return new SatisfaccionResponseDto
            {
                PromedioComida = resumen.PromedioComida,
                PromedioLugar = resumen.PromedioLugar,
                PromedioAtencion = resumen.PromedioAtencion,
                TotalEncuestas = resumen.TotalEncuestas,
                TotalDerivadosGoogleMaps = resumen.TotalDerivadosGoogleMaps,
                PorcentajeDerivados = resumen.PorcentajeDerivados
            };
        }
    }
}
