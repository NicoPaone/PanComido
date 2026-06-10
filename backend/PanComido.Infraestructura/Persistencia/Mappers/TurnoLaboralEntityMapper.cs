using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;
namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class TurnoLaboralEntityMapper
    {
        public DOM.TurnoLaboral paraDominio(EF.TurnoLaboral efTurnoLaboral)
        {
            return new DOM.TurnoLaboral
            {
                Id = efTurnoLaboral.Id,
                RestauranteId = efTurnoLaboral.RestauranteId,
                HorarioInicio = efTurnoLaboral.HorarioLaboralInicio,
                HorarioFin = efTurnoLaboral.HorarioLaboralFin,
                EsNocturno = efTurnoLaboral.EsNocturno
            };
        }

        public void paraActualizarEntidad(EF.TurnoLaboral efTurnosExistentes, DOM.TurnoLaboral turnosNuevos)
        {
            efTurnosExistentes.HorarioLaboralInicio = turnosNuevos.HorarioInicio;
            efTurnosExistentes.HorarioLaboralFin = turnosNuevos.HorarioFin;
            efTurnosExistentes.EsNocturno = turnosNuevos.EsNocturno;
        }
    }
}
