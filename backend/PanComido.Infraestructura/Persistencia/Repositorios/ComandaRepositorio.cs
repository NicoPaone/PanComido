using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Entidades;
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

        public async Task<int> CrearAsync(DOM.Comanda comandaDominio)
        {
            EF.Comandum comandaEF = _mapper.paraEntidad(comandaDominio);
            comandaEF.HoraUltimoCambioEstado = DateTime.Now;

            await _ctx.Comanda.AddAsync(comandaEF);

            await _ctx.SaveChangesAsync();

            return comandaEF.Id;
        }

        public async Task<DOM.Comanda?> ModificarEstadoComandaAsync(int mesaId, int estadoId)
        {

            Console.WriteLine("modificar en repoo");
            var efComanda = await _ctx.Comanda
               .FirstOrDefaultAsync(m => m.MesaId == mesaId
               && m.EstadoComandaId != (int)EstadoComanda.Finalizada
               && m.EstadoComandaId != (int)EstadoComanda.Abierta);
            Console.WriteLine("El objeto: " + efComanda);

            if (efComanda == null)
                return null;
            // esto deberia hacerlo el dominio
            efComanda.EstadoComandaId = estadoId;
            efComanda.HoraUltimoCambioEstado = DateTime.Now;

            await _ctx.SaveChangesAsync();

            return _mapper.ParaDominio(efComanda);
        }
        public async Task<DOM.Comanda?> ObtenerComandaPorIdMesaAsync(int mesaId)
        {
            var efComanda = await _ctx.Comanda
            .Include(c => c.ArticuloComanda.Where(ac => ac.Articulo.Plato != null))
            .ThenInclude(ac => ac.Articulo)
            .ThenInclude(a => a.Plato).FirstOrDefaultAsync(m => m.MesaId == mesaId);
            ;

            return efComanda == null ? null : _mapper.ParaDominio(efComanda);
        }

        public async Task<List<Comanda>> ObtenerComandasActivasParaCocinaAsync(int restauranteId)
        {
            var efList = await _ctx.Comanda
               .Include(c => c.EstadoComanda)
               .Include(c => c.ArticuloComanda.Where(ac => ac.Articulo.Plato != null))
                   .ThenInclude(ac => ac.Articulo)
                       .ThenInclude(a => a.Plato)
               .Where(c => c.RestauranteId == restauranteId)
               .Where(c => c.EstadoComandaId != (int)EstadoComanda.Finalizada
                           && c.EstadoComandaId != (int)EstadoComanda.Abierta)
               .ToListAsync();
            return efList.Select(C => _mapper.ParaDominio(C)).ToList();
        }

        private IQueryable<EF.Comandum> BaseQueryMozo()
        {
            return _ctx.Comanda
                .Include(c => c.EstadoComanda)
                .Include(c => c.ArticuloComanda.Where(ac => ac.Articulo.ConfiguracionArticulos.Any(ca => ca.Id == 1)))
                    .ThenInclude(ac => ac.Articulo)
                        .ThenInclude(a => a.Plato)
                .Include(c => c.ArticuloComanda.Where(ac => ac.Articulo.ConfiguracionArticulos.Any(ca => ca.Id == 1)))
                    .ThenInclude(ac => ac.Articulo)
                        .ThenInclude(a => a.Insumo)
                .Include(c => c.Mesa)
                    .ThenInclude(m => m.Mozos);
        }

        public async Task<List<Comanda>> ObtenerComandasActivasPorMozoAsync(int restauranteId, int mozoId)
        {
            var efList = await BaseQueryMozo()
               .Where(c => c.RestauranteId == restauranteId)
               .Where(c => _ctx.Mozos
                   .Where(m => m.IdEmpleado == mozoId)
                   .SelectMany(m => m.Mesas)
                   .Any(mesa => mesa.Id == c.MesaId))
               .Where(c => c.EstadoComandaId != (int)EstadoComanda.Finalizada
                           && c.EstadoComandaId != (int)EstadoComanda.Abierta)
               .ToListAsync();
            return efList.Select(c => _mapper.ParaDominio(c)).ToList();
        }

        public async Task<DOM.Comanda?> ObtenerComandaPorIdAsync(int comandaId)
        {
            var efComanda = await BaseQueryMozo()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == comandaId);

            return efComanda == null ? null : _mapper.ParaDominio(efComanda);
        }

        public async Task MarcarItemsEntregadosAsync(int comandaId, List<int> articuloComandaIds)
        {
            var efItems = await _ctx.ArticuloComanda
                .Where(ac => ac.ComandaId == comandaId && articuloComandaIds.Contains(ac.Id))
                .ToListAsync();

            foreach (var item in efItems)
                item.Entregado = true;

            await _ctx.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Comanda comanda)
        {
            var efComanda = _mapper.paraEntidad(comanda);
            _ctx.Comanda.Update(efComanda);
            await _ctx.SaveChangesAsync();
        }

        public async Task ActualizarAsync(DOM.Comanda comanda)
        {
            var efComanda = await _ctx.Comanda
                .FirstOrDefaultAsync(c => c.Id == comanda.Id);

            if (efComanda == null) return;
            var efActualizado = _mapper.paraEntidad(comanda);

            efComanda.EstadoComandaId = efActualizado.EstadoComandaId;
            efComanda.PagoId = efActualizado.PagoId;
            efComanda.HoraFin = efActualizado.HoraFin;
            efComanda.HoraUltimoCambioEstado = DateTime.Now;

            await _ctx.SaveChangesAsync();
        }
    }

}




