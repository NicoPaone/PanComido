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

        public async Task<DOM.Comanda?> ModificarEstadoComandaAsync(int comandaId, int estadoId)
        {

            var efComanda = await _ctx.Comanda
               .FirstOrDefaultAsync(m => m.Id == comandaId
               && m.EstadoComandaId != (int)EstadoComanda.Finalizada
               && m.EstadoComandaId != (int)EstadoComanda.Abierta);

            if (efComanda == null)
                return null;

            efComanda.EstadoComandaId = estadoId;
            efComanda.HoraUltimoCambioEstado = DateTime.Now;

            await _ctx.SaveChangesAsync();

            return _mapper.ParaDominio(efComanda);
        }
        public async Task<DOM.Comanda?> ObtenerComandaPorIdMesaAsync(int mesaId)
        {
            var efComanda = await BaseQueryCocina()
                               .FirstOrDefaultAsync(m => m.MesaId == mesaId
                               && m.EstadoComandaId != (int)EstadoComanda.Finalizada);
            return efComanda == null ? null : _mapper.ParaDominio(efComanda);
        }

        public async Task<List<Comanda>> ObtenerComandasActivasParaCocinaAsync(int restauranteId)
        {
            var efList = await BaseQueryCocina()
                            .Where(c => c.RestauranteId == restauranteId
                                     && c.EstadoComandaId != (int)EstadoComanda.Finalizada
                                     && c.EstadoComandaId != (int)EstadoComanda.Abierta
                                     && c.ArticuloComanda.Any(ac => ac.Articulo.Plato != null))
                            .ToListAsync();

            return efList.Select(C => _mapper.ParaDominio(C)).ToList();
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

        public async Task ActualizarComandaParaPagoAsync(DOM.Comanda comanda)
        {
            var efComanda = await _ctx.Comanda
                .FirstOrDefaultAsync(c => c.Id == comanda.Id);

            if (efComanda == null) return;
            var efActualizado = _mapper.paraEntidad(comanda);

            efComanda.EstadoComandaId = efActualizado.EstadoComandaId;
            efComanda.HoraFin = efActualizado.HoraFin;
            efComanda.HoraUltimoCambioEstado = DateTime.Now;

            await _ctx.SaveChangesAsync();
        }

        private IQueryable<EF.Comandum> BaseQueryMozo()
        {
            return _ctx.Comanda
                .Include(c => c.EstadoComanda)
                .Include(c => c.ArticuloComanda)
                    .ThenInclude(ac => ac.Articulo)
                        .ThenInclude(a => a.Plato)
                .Include(c => c.ArticuloComanda)
                    .ThenInclude(ac => ac.Articulo)
                        .ThenInclude(a => a.Insumo)
                .Include(c => c.ArticuloComanda)
                    .ThenInclude(ac => ac.ArticuloComandaIngredienteExcluidos)
                      .ThenInclude(ex => ex.Ingrediente)
                        .ThenInclude(i => i.IdInsumoNavigation)
                            .ThenInclude(ins => ins.IdArticuloNavigation)
                .Include(c => c.Mesa)
                    .ThenInclude(m => m.Mozos);
        }

        private IQueryable<EF.Comandum> BaseQueryCocina()
        {
            return _ctx.Comanda
                .Include(c => c.EstadoComanda)
                .Include(c => c.Mesa)
                .Include(c => c.ArticuloComanda.Where(ac => ac.Articulo.Plato != null))
                    .ThenInclude(ac => ac.Articulo)
                        .ThenInclude(a => a.Plato)
                .Include(c => c.ArticuloComanda)
                    .ThenInclude(ac => ac.ArticuloComandaIngredienteExcluidos)
                        .ThenInclude(ex => ex.Ingrediente)
                            .ThenInclude(i => i.IdInsumoNavigation)
                                .ThenInclude(ins => ins.IdArticuloNavigation);
        }

        public async Task<List<DOM.VentaReporteDetalle>> ObtenerReporteVentasPorPeriodoAsync(int restauranteId, DateTime desde, DateTime hasta)
        {
            return await _ctx.Comanda
                .Where(c => c.RestauranteId == restauranteId 
                         && c.HoraInicio >= desde 
                         && c.HoraInicio <= hasta
                         && c.Pagos.Any())
                .OrderByDescending(c => c.HoraInicio)
                .Select(c => new DOM.VentaReporteDetalle
                {
                    ComandaId = c.Id,
                    NumeroMesa = c.Mesa != null ? c.Mesa.Numero : 0,
                    FechaHora = c.HoraInicio,
                    CantidadArticulos = c.ArticuloComanda.Sum(ac => ac.Cantidad),
                    Total = c.Pagos.Sum(p => p.Total),
                    MetodoPago = c.Pagos
                        .OrderBy(p => p.Id)
                        .Select(p => p.MetodoPago != null ? p.MetodoPago.Descripcion : "Otro")
                        .FirstOrDefault() ?? "Otro"
                })
                .ToListAsync();
        }
    }
}



