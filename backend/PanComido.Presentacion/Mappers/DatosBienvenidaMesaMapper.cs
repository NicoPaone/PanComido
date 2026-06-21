using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.Mesas;

namespace PanComido.Presentacion.Mappers
{
    public class DatosBienvenidaMesaMapper
    {
            public BienvenidaMesaResponseDto aDto(BienvenidaMesaDatos datosBienvenidaDominio)
            {
                return new BienvenidaMesaResponseDto
                {
                    IdMesa = datosBienvenidaDominio.Mesa.Id,
                    NumeroMesa = datosBienvenidaDominio.Mesa.Numero,
                    CantidadMaximaComensales = datosBienvenidaDominio.Mesa.CantPersonasMax,
                    EstadoActual = datosBienvenidaDominio.Mesa.EstadoMesa.ToString(),
                    RestauranteId = datosBienvenidaDominio.RestauranteId
                };
        }
    }
}
