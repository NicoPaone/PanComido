using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Interfaces.Repositorios;
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

        public async Task<DOM.Mesa?> ObtenerPorIdAsync(int id, int restauranteId)
        {
            EF.Mesa mesaEF = await BaseQuery(restauranteId)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            return _mapper.paraDominio(mesaEF);
        }

        public async Task ActualizarAsync(DOM.Mesa mesaDominio)
        {
            EF.Mesa mesaEF = _mapper.paraEntidad(mesaDominio);

            _ctx.Mesas.Update(mesaEF);

            await _ctx.SaveChangesAsync();
        }

        
    }
}
