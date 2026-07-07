
using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
   public class EmpleadoRepositorio : IEmpleadoRepositorio
   {
      private readonly AppDbContext _ctx;

      public EmpleadoRepositorio(AppDbContext ctx)
      {
         _ctx = ctx;
      }

      public async Task<Empleado?> ObtenerPorEmailAsync(string email)
      {
         var entidad = await _ctx.Empleados
            .Include(e => e.Mozo)
            .Include(e => e.Gerente)
            .Include(e => e.Cocina)
            .FirstOrDefaultAsync(e => e.Email == email && !e.Eliminado);

         if (entidad == null) return null;

         string rol = "";
         if (entidad.Mozo != null) rol = RolEmpleado.Mozo;
         else if (entidad.Gerente != null) rol = RolEmpleado.Gerente;
         else if (entidad.Cocina != null) rol = RolEmpleado.Cocina;

         return new Empleado
         {
            Id = entidad.Id,
            RestauranteId = entidad.RestauranteId,
            Nombre = entidad.Nombre,
            Email = entidad.Email,
            ContraseniaHash = entidad.Contrasena,
            Estado = entidad.Estado,
            Rol = rol
         };
      }

      public async Task<string?> ObtenerRolAsync(int empleadoId)
      {
         if (await _ctx.Mozos.AnyAsync(m    =>     m.IdEmpleado == empleadoId))  return RolEmpleado.Mozo;
         if (await _ctx.Gerentes.AnyAsync(g =>     g.IdEmpleado == empleadoId))  return RolEmpleado.Gerente;
         if (await _ctx.Cocinas.AnyAsync(c  =>     c.IdEmpleado == empleadoId))  return RolEmpleado.Cocina;
         
         return null;
      }

      public async Task<List<Empleado>> ObtenerTodosPorRestauranteAsync(int restauranteId)
      {
         var entidades = await _ctx.Empleados
            .Include(e => e.TurnoLaborals)
            .Include(e => e.Mozo)
            .Include(e => e.Gerente)
            .Include(e => e.Cocina)
            .Where(e => e.RestauranteId == restauranteId && !e.Eliminado)
            .ToListAsync();

         return entidades.Select(entidad =>
         {
            string rol = "";
            if (entidad.Mozo != null) rol = RolEmpleado.Mozo;
            else if (entidad.Gerente != null) rol = RolEmpleado.Gerente;
            else if (entidad.Cocina != null) rol = RolEmpleado.Cocina;

            return new Empleado
            {
               Id = entidad.Id,
               RestauranteId = entidad.RestauranteId,
               Nombre = entidad.Nombre,
               Email = entidad.Email,
               ContraseniaHash = entidad.Contrasena,
               Estado = entidad.Estado,
               Rol = rol,
               Turnos = entidad.TurnoLaborals.Select(t => new TurnoLaboral
               {
                  Id = t.Id,
                  RestauranteId = t.RestauranteId,
                  HorarioInicio = t.HorarioLaboralInicio,
                  HorarioFin = t.HorarioLaboralFin,
                  EsNocturno = t.EsNocturno
               }).ToList()
            };
         }).ToList();
      }

      public async Task<Empleado?> ObtenerPorIdYRestauranteAsync(int id, int restauranteId)
      {
         var entidad = await _ctx.Empleados
            .Include(e => e.TurnoLaborals)
            .Include(e => e.Mozo)
            .Include(e => e.Gerente)
            .Include(e => e.Cocina)
            .FirstOrDefaultAsync(e => e.Id == id && e.RestauranteId == restauranteId && !e.Eliminado);

         if (entidad == null) return null;

         string rol = "";
         if (entidad.Mozo != null) rol = RolEmpleado.Mozo;
         else if (entidad.Gerente != null) rol = RolEmpleado.Gerente;
         else if (entidad.Cocina != null) rol = RolEmpleado.Cocina;

         return new Empleado
         {
            Id = entidad.Id,
            RestauranteId = entidad.RestauranteId,
            Nombre = entidad.Nombre,
            Email = entidad.Email,
            ContraseniaHash = entidad.Contrasena,
            Estado = entidad.Estado,
            Rol = rol,
            Turnos = entidad.TurnoLaborals.Select(t => new TurnoLaboral
            {
               Id = t.Id,
               RestauranteId = t.RestauranteId,
               HorarioInicio = t.HorarioLaboralInicio,
               HorarioFin = t.HorarioLaboralFin,
               EsNocturno = t.EsNocturno
            }).ToList()
         };
      }

      public async Task CrearAsync(Empleado empleado, List<int> turnosIds)
      {
         var efEmpleado = new EF.Empleado
         {
            RestauranteId = empleado.RestauranteId,
            Nombre = empleado.Nombre,
            Email = empleado.Email,
            Contrasena = empleado.ContraseniaHash,
            Estado = empleado.Estado,
            Eliminado = false
         };

         if (turnosIds != null && turnosIds.Any())
         {
            var turnos = await _ctx.TurnoLaborals
               .Where(t => t.RestauranteId == empleado.RestauranteId && turnosIds.Contains(t.Id))
               .ToListAsync();
            foreach (var turno in turnos)
            {
               efEmpleado.TurnoLaborals.Add(turno);
            }
         }

         if (empleado.Rol.Equals(RolEmpleado.Mozo, System.StringComparison.OrdinalIgnoreCase))
         {
            efEmpleado.Mozo = new EF.Mozo { Activo = true };
         }
         else if (empleado.Rol.Equals(RolEmpleado.Gerente, System.StringComparison.OrdinalIgnoreCase))
         {
            efEmpleado.Gerente = new EF.Gerente();
         }
         else if (empleado.Rol.Equals(RolEmpleado.Cocina, System.StringComparison.OrdinalIgnoreCase))
         {
            efEmpleado.Cocina = new EF.Cocina();
         }

         _ctx.Empleados.Add(efEmpleado);
         await _ctx.SaveChangesAsync();
         empleado.Id = efEmpleado.Id;
      }

      public async Task ModificarAsync(Empleado empleado, List<int> turnosIds)
      {
         var efEmpleado = await _ctx.Empleados
            .Include(e => e.TurnoLaborals)
            .Include(e => e.Mozo)
            .Include(e => e.Gerente)
            .Include(e => e.Cocina)
            .FirstOrDefaultAsync(e => e.Id == empleado.Id && e.RestauranteId == empleado.RestauranteId && !e.Eliminado);

         if (efEmpleado == null) return;

         efEmpleado.Nombre = empleado.Nombre;
         efEmpleado.Email = empleado.Email;
         efEmpleado.Estado = empleado.Estado;

         if (!string.IsNullOrEmpty(empleado.ContraseniaHash))
         {
            efEmpleado.Contrasena = empleado.ContraseniaHash;
         }

         efEmpleado.TurnoLaborals.Clear();
         if (turnosIds != null && turnosIds.Any())
         {
            var turnos = await _ctx.TurnoLaborals
               .Where(t => t.RestauranteId == empleado.RestauranteId && turnosIds.Contains(t.Id))
               .ToListAsync();
            foreach (var turno in turnos)
            {
               efEmpleado.TurnoLaborals.Add(turno);
            }
         }

         string rolActual = "";
         if (efEmpleado.Mozo != null) rolActual = RolEmpleado.Mozo;
         else if (efEmpleado.Gerente != null) rolActual = RolEmpleado.Gerente;
         else if (efEmpleado.Cocina != null) rolActual = RolEmpleado.Cocina;

         if (!rolActual.Equals(empleado.Rol, System.StringComparison.OrdinalIgnoreCase))
         {
            if (efEmpleado.Mozo != null) _ctx.Mozos.Remove(efEmpleado.Mozo);
            if (efEmpleado.Gerente != null) _ctx.Gerentes.Remove(efEmpleado.Gerente);
            if (efEmpleado.Cocina != null) _ctx.Cocinas.Remove(efEmpleado.Cocina);

            efEmpleado.Mozo = null;
            efEmpleado.Gerente = null;
            efEmpleado.Cocina = null;

            if (empleado.Rol.Equals(RolEmpleado.Mozo, System.StringComparison.OrdinalIgnoreCase))
            {
               efEmpleado.Mozo = new EF.Mozo { Activo = true };
            }
            else if (empleado.Rol.Equals(RolEmpleado.Gerente, System.StringComparison.OrdinalIgnoreCase))
            {
               efEmpleado.Gerente = new EF.Gerente();
            }
            else if (empleado.Rol.Equals(RolEmpleado.Cocina, System.StringComparison.OrdinalIgnoreCase))
            {
               efEmpleado.Cocina = new EF.Cocina();
            }
         }

         await _ctx.SaveChangesAsync();
      }

      public async Task EliminarLogicoAsync(int id, int restauranteId)
      {
         var efEmpleado = await _ctx.Empleados
            .FirstOrDefaultAsync(e => e.Id == id && e.RestauranteId == restauranteId && !e.Eliminado);

         if (efEmpleado != null)
         {
            efEmpleado.Eliminado = true;
            await _ctx.SaveChangesAsync();
         }
      }
   }
}
