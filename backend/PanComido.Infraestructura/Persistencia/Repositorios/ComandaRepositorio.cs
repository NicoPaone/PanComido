using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;


namespace PanComido.Infraestructura.Persistencia.Repositorios
{
   public class ComandaRepositorio : IComandaRepositorio
   {
      private readonly AppDbContext _ctx;
      private readonly ComandaEntityMapper _mapper;
      public ComandaRepositorio(AppDbContext context, ComandaEntityMapper mapper)
      {
         _ctx = context;
         _mapper = mapper;
      }

        public async Task CrearAsync(DOM.Comanda comandaDominio)
        {
            EF.Comandum comandaEF = _mapper.paraEntidad(comandaDominio);

            await _ctx.Comanda.AddAsync(comandaEF);

            await _ctx.SaveChangesAsync();
        }

        public async Task<DOM.Comanda?> ModificarEstadoComandaAsync(int mesaId, int estadoId)
      {

         Console.WriteLine("modificar en repoo");
         var efComanda = await _ctx.Comanda
            .FirstOrDefaultAsync(m => m.MesaId == mesaId 
            && m.EstadoComandaId != (int)EstadoComanda.Finalizada
            && m.EstadoComandaId != (int)EstadoComanda.Abierta);
         Console.WriteLine("El objeto: "+ efComanda);

         if (efComanda == null)
            return null;
         // esto deberia hacerlo el dominio
         efComanda.EstadoComandaId = estadoId;

         await _ctx.SaveChangesAsync();

         return _mapper.ParaDominio(efComanda);
      }
      public async Task<DOM.Comanda?> ObtenerComandaPorIdMesaAsync(int mesaId)
      {
         var efComanda = await _ctx.Comanda.FirstOrDefaultAsync(m => m.MesaId == mesaId);

         return efComanda == null ? null : _mapper.ParaDominio(efComanda);
      }


      public async Task<List<Comanda>> ObtenerComandasActivasAsync(int restauranteId)
      {
         var efList = await  _ctx.Comanda
            .Include(c => c.EstadoComanda)
            .Include(c => c.ArticuloComanda)
            .ThenInclude(ac => ac.Articulo)
            .ThenInclude (a => a.Plato)
            .Where(c => c.RestauranteId == restauranteId)
            .Where(c => c.EstadoComandaId != (int)EstadoComanda.Finalizada
                        && c.EstadoComandaId != (int)EstadoComanda.Abierta)
            .ToListAsync();
         return  efList.Select(C=> _mapper.ParaDominio(C)).ToList();
      }

      
   }

} 


    

