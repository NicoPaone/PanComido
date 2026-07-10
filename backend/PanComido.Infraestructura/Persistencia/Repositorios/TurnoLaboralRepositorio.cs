using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class TurnoLaboralRepositorio : ITurnoLaboralRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly TurnoLaboralEntityMapper _turnoLaboralMapper;

        public TurnoLaboralRepositorio(AppDbContext ctx, TurnoLaboralEntityMapper turnoLaboralMapper)
        {
            _ctx = ctx;
            _turnoLaboralMapper = turnoLaboralMapper;
        }

        public async Task<List<TurnoLaboral>> ActualizarTurnosLaboralesAsync(int restauranteId, List<TurnoLaboral> turnos)
        {
            var efTurnos = await _ctx.TurnoLaborals
                .Where(t => t.RestauranteId == restauranteId)
                .ToListAsync();

            foreach (var turno in turnos)
            {
                var efTurno = efTurnos.FirstOrDefault(t => t.Id == turno.Id);
                if (efTurno == null)
                {
                    efTurno = new EF.TurnoLaboral
                    {
                        RestauranteId = restauranteId,
                        HorarioLaboralInicio = turno.HorarioInicio,
                        HorarioLaboralFin = turno.HorarioFin,
                        EsNocturno = turno.EsNocturno
                    };
                    await _ctx.TurnoLaborals.AddAsync(efTurno);
                    efTurnos.Add(efTurno);
                }
                else
                {
                    _turnoLaboralMapper.paraActualizarEntidad(efTurno, turno);
                }
            }

            await _ctx.SaveChangesAsync();
            return efTurnos.Select(t => _turnoLaboralMapper.paraDominio(t)).ToList();
        }

        public async Task<List<TurnoLaboral>> ObtenerTurnosLaboralesAsync(int restauranteId)
        {
            var efTurnoLaboral = await _ctx.TurnoLaborals
                .Where(t => t.RestauranteId == restauranteId)
                .ToListAsync();

            return efTurnoLaboral.Select(t => _turnoLaboralMapper.paraDominio(t)).ToList();
        }
    }
}