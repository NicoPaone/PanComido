using PanComido.Presentacion.DTOs.TurnoLaboral;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class TurnoLaboralMapper
    {
        public TurnoLaboralResponseDto aDto(DOM.TurnoLaboral turnoLaboral)
        {
            return new TurnoLaboralResponseDto
            {
                Id = turnoLaboral.Id,
                RestauranteId = turnoLaboral.RestauranteId,
                HorarioInicio = turnoLaboral.HorarioInicio,
                HorarioFin = turnoLaboral.HorarioFin,
                EsNocturno = turnoLaboral.EsNocturno
            };
        }

        public List<TurnoLaboralResponseDto> aListaDto(List<DOM.TurnoLaboral> turnosLaborales)
        {
            return turnosLaborales.Select(t => aDto(t)).ToList();
        }

        public DOM.TurnoLaboral aDominio(TurnoLaboralRequestDto turnoLaboralRequest)
        {
            return new DOM.TurnoLaboral
            {
                Id = turnoLaboralRequest.Id,
                HorarioInicio = turnoLaboralRequest.HorarioInicio,
                HorarioFin = turnoLaboralRequest.HorarioFin,
                EsNocturno = turnoLaboralRequest.EsNocturno
            };
        }

        public List<DOM.TurnoLaboral> aListaDominio(List<TurnoLaboralRequestDto> turnosLaborales)
        {
            return turnosLaborales.Select(t => aDominio(t)).ToList();
        }
    }
}
