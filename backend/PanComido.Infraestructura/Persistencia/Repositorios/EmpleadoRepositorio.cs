
using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;

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
         var entidad = await _ctx.Empleados.FirstOrDefaultAsync(e => e.Email == email);

         if (entidad == null) return null;

         return new Empleado
         {
            Id = entidad.Id,
            RestauranteId = entidad.RestauranteId,
            Nombre = entidad.Nombre,
            Email = entidad.Email,
            ContraseniaHash = entidad.Contrasena,
            Estado = entidad.Estado

         };
      }

      public async Task<string?> ObtenerRolAsync(int empleadoId)
      {
         if (await _ctx.Mozos.AnyAsync(m    =>     m.IdEmpleado == empleadoId))  return "Mozo";
         if (await _ctx.Gerentes.AnyAsync(g =>     g.IdEmpleado == empleadoId))  return "Gerente";
         if (await _ctx.Cocinas.AnyAsync(c  =>     c.IdEmpleado == empleadoId))  return "Cocina";
         
         return null;
      }
   }
}
