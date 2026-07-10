using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Linq;
using System.Threading.Tasks;

using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class TurnoFilaRepositorio : ITurnoFilaRepositorio
    {
        private readonly AppDbContext _ctx;

        public TurnoFilaRepositorio(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task CrearAsync(DOM.TurnoFila turno)
        {
            var efTurno = new EF.TurnoFila
            {
                FilaVirtualId = turno.FilaVirtualId,
                Numero = turno.Numero,
                CantidadComensales = turno.CantidadComensales,
                FechaHoraIngreso = turno.FechaHoraIngreso,
                EstadoTurnoMesaId = (int)turno.Estado,
                MesaAsignadaId = turno.MesaAsignadaId,
                FechaHoraAsignacion = turno.FechaHoraAsignacion,
                ComandaPreArmadaId = turno.ComandaPreArmadaId
            };

            _ctx.TurnoFilas.Add(efTurno);
            await _ctx.SaveChangesAsync();

            turno.Id = efTurno.Id;
        }

        public async Task<DOM.TurnoFila> ObtenerPorIdAsync(int id)
        {
            var efTurno = await _ctx.TurnoFilas
                .FirstOrDefaultAsync(t => t.Id == id);

            if (efTurno == null) return null;

            return new DOM.TurnoFila
            {
                Id = efTurno.Id,
                FilaVirtualId = efTurno.FilaVirtualId,
                Numero = efTurno.Numero,
                CantidadComensales = efTurno.CantidadComensales,
                FechaHoraIngreso = efTurno.FechaHoraIngreso,
                Estado = (EstadoTurnoMesa)efTurno.EstadoTurnoMesaId,
                MesaAsignadaId = efTurno.MesaAsignadaId,
                FechaHoraAsignacion = efTurno.FechaHoraAsignacion,
                ComandaPreArmadaId = efTurno.ComandaPreArmadaId
            };
        }

        public async Task<int> ObtenerProximoNumeroTurnoAsync(int filaVirtualId)
        {
            var maxNumero = await _ctx.TurnoFilas
                .Where(t => t.FilaVirtualId == filaVirtualId)
                .MaxAsync(t => (int?)t.Numero) ?? 0;
            return maxNumero + 1;
        }

        public async Task<int> ContarTurnosEnEsperaPreviosAsync(int filaVirtualId, DateTime fechaHoraIngreso)
        {
            return await _ctx.TurnoFilas
                .Where(t => t.FilaVirtualId == filaVirtualId &&
                            t.EstadoTurnoMesaId == (int)EstadoTurnoMesa.EnEspera &&
                            t.FechaHoraIngreso < fechaHoraIngreso)
                .CountAsync();
        }

        public async Task<int> ObtenerFilaVirtualIdAsync(int restauranteId)
        {
            var filaVirtual = await _ctx.FilaVirtuals
                .Where(f => f.RestauranteId == restauranteId)
                .FirstOrDefaultAsync();

            if (filaVirtual == null)
            {
                throw new ArgumentException($"No se encontró una fila virtual para el restaurante {restauranteId}.");
            }

            return filaVirtual.Id;
        }

        public async Task ActualizarAsync(DOM.TurnoFila turno)
        {
            var efTurno = await _ctx.TurnoFilas.FirstOrDefaultAsync(t => t.Id == turno.Id);
            if (efTurno != null)
            {
                efTurno.EstadoTurnoMesaId = (int)turno.Estado;
                efTurno.MesaAsignadaId = turno.MesaAsignadaId;
                efTurno.FechaHoraAsignacion = turno.FechaHoraAsignacion;
                efTurno.ComandaPreArmadaId = turno.ComandaPreArmadaId;

                await _ctx.SaveChangesAsync();
            }
        }

        public async Task<DOM.FilaVirtual> ObtenerFilaVirtualPorIdAsync(int filaVirtualId)
        {
            var efFila = await _ctx.FilaVirtuals.FirstOrDefaultAsync(f => f.Id == filaVirtualId);
            if (efFila == null) return null;

            return new DOM.FilaVirtual
            {
                Id = efFila.Id,
                RestauranteId = efFila.RestauranteId,
                Habilitada = efFila.Habilitada,
                TiempoPromedioComidaMinutos = efFila.TiempoPromedioComidaMinutos
            };
        }

        public async Task<System.Collections.Generic.List<DOM.TurnoFila>> ObtenerTurnosAsignadosExpiradosAsync(DateTime fechaLimite)
        {
            var turnosEf = await _ctx.TurnoFilas
                .Where(t => t.EstadoTurnoMesaId == (int)EstadoTurnoMesa.MesaAsignada
                         && t.FechaHoraAsignacion.HasValue
                         && t.FechaHoraAsignacion.Value < fechaLimite)
                .ToListAsync();

            return turnosEf.Select(efTurno => new DOM.TurnoFila
            {
                Id = efTurno.Id,
                FilaVirtualId = efTurno.FilaVirtualId,
                Numero = efTurno.Numero,
                CantidadComensales = efTurno.CantidadComensales,
                FechaHoraIngreso = efTurno.FechaHoraIngreso,
                Estado = (EstadoTurnoMesa)efTurno.EstadoTurnoMesaId,
                MesaAsignadaId = efTurno.MesaAsignadaId,
                FechaHoraAsignacion = efTurno.FechaHoraAsignacion,
                ComandaPreArmadaId = efTurno.ComandaPreArmadaId
            }).ToList();
        }

        public async Task<DOM.TurnoFila?> ObtenerProximoTurnoEnEsperaAsync(int filaVirtualId, int capacidadMesa)
        {
            var efTurno = await _ctx.TurnoFilas
                .Where(t => t.FilaVirtualId == filaVirtualId 
                         && t.EstadoTurnoMesaId == (int)EstadoTurnoMesa.EnEspera
                         && t.CantidadComensales <= capacidadMesa)
                .OrderBy(t => t.FechaHoraIngreso)
                .FirstOrDefaultAsync();

            if (efTurno == null) return null;

            return new DOM.TurnoFila
            {
                Id = efTurno.Id,
                FilaVirtualId = efTurno.FilaVirtualId,
                Numero = efTurno.Numero,
                CantidadComensales = efTurno.CantidadComensales,
                FechaHoraIngreso = efTurno.FechaHoraIngreso,
                Estado = (EstadoTurnoMesa)efTurno.EstadoTurnoMesaId,
                MesaAsignadaId = efTurno.MesaAsignadaId,
                FechaHoraAsignacion = efTurno.FechaHoraAsignacion,
                ComandaPreArmadaId = efTurno.ComandaPreArmadaId
            };
        }
    }
}
