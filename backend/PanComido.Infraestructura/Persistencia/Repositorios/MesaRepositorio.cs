using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Entidades;
using PanComido.Infraestructura.Persistencia.Mappers;

using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
   public class MesaRepositorio : IMesaRepositorio
   {
      private readonly AppDbContext _ctx;
      private readonly MesaEntityMapper _mapper;
      public MesaRepositorio(AppDbContext ctx, MesaEntityMapper mapper)
      {
         _ctx = ctx;
         _mapper = mapper;
      }
      private IQueryable<EF.Mesa> BaseQuery(int restauranteId)
      {
         return _ctx.Mesas
            .Where(m => m.Grilla.RestauranteId == restauranteId);
      }
      public async Task<DOM.MesaConPosiciones?> ObtenerPorIdAsync(int id, int restauranteId)
      {
         EF.Mesa mesaEF = await BaseQuery(restauranteId)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
         return _mapper.paraDominioCompleto(mesaEF);
      }
      public async Task ActualizarAsync(DOM.Mesa mesaDominio)
      {
         EF.Mesa mesaEF = _mapper.paraEntidad(mesaDominio);
         _ctx.Mesas.Update(mesaEF);
         await _ctx.SaveChangesAsync();
      }
      public async Task<List<MesaConPosiciones>> ObtenerTodasAsync(int restauranteId)
      {
         List<EF.Mesa> mesasEF = await BaseQuery(restauranteId)
            .AsNoTracking()
            .Include(m => m.DimensionMesa)
            .ToListAsync();

         return mesasEF
            .Select(m => _mapper.paraDominioCompleto(m)!)
            .Where(m => m != null)
            .ToList();
      }

      public async Task ActualizarEstadoAsync(int mesaId, DOM.Enums.EstadoMesa nuevoEstado)
      {
         var mesaEF = await _ctx.Mesas.FirstOrDefaultAsync( m => m.Id == mesaId );
         if (mesaEF == null) return;

         mesaEF.EstadoMesaId = (int)nuevoEstado;
         await _ctx.SaveChangesAsync();
      }
   }
}
