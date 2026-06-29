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
            var efComanda = await _ctx.Comanda
            .Include(c => c.Mesa)
            .Include(c => c.ArticuloComanda.Where(ac => ac.Articulo.Plato != null))
                .ThenInclude(ac => ac.Articulo)
                    .ThenInclude(a => a.Plato)
            .Include(c => c.ArticuloComanda)
                .ThenInclude(ac => ac.ArticuloComandaIngredienteExcluidos)
                    .ThenInclude(ex => ex.Ingrediente)
                        .ThenInclude(i => i.IdInsumoNavigation)
                           .ThenInclude(ins => ins.IdArticuloNavigation)
            .FirstOrDefaultAsync(m => m.MesaId == mesaId 
                                   && m.EstadoComandaId != (int)EstadoComanda.Finalizada);

            return efComanda == null ? null : _mapper.ParaDominio(efComanda);
        }

        public async Task<List<Comanda>> ObtenerComandasActivasParaCocinaAsync(int restauranteId)
        {
            var efList = await _ctx.Comanda
               .Include(c => c.EstadoComanda)
               .Include(c => c.ArticuloComanda.Where(ac => ac.Articulo.Plato != null))
                   .ThenInclude(ac => ac.Articulo)
                       .ThenInclude(a => a.Plato)
               .Include(c => c.ArticuloComanda)
                   .ThenInclude(ac => ac.ArticuloComandaIngredienteExcluidos)
                      .ThenInclude(ex => ex.Ingrediente)
                       .ThenInclude(i => i.IdInsumoNavigation)
                           .ThenInclude(ins => ins.IdArticuloNavigation)
               .Include(c => c.Mesa)
               .Where(c => c.RestauranteId == restauranteId)
               .Where(c => c.EstadoComandaId != (int)EstadoComanda.Finalizada
                           && c.EstadoComandaId != (int)EstadoComanda.Abierta)
               .Where(c => c.ArticuloComanda.Any(ac => ac.Articulo.Plato != null))
               .ToListAsync();
            return efList.Select(C => _mapper.ParaDominio(C)).ToList();
        }

        private IQueryable<EF.Comandum> BaseQueryMozo()
        {
            return _ctx.Comanda
                .Include(c => c.EstadoComanda)
                .Include(c => c.ArticuloComanda.Where(ac => ac.Articulo.ConfiguracionArticulos.Any(ca => ca.Id == (int)ConfiguracionArticuloEnum.Vendible)))
                    .ThenInclude(ac => ac.Articulo)
                        .ThenInclude(a => a.Plato)
                .Include(c => c.ArticuloComanda.Where(ac => ac.Articulo.ConfiguracionArticulos.Any(ca => ca.Id == (int)ConfiguracionArticuloEnum.Vendible)))
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

        private IQueryable<DOM.RendimientoPlato> BaseQueryRendimiento(int restauranteId, DateTime fechaDesde, DateTime fechaHasta)
        {
            return _ctx.Articulos
                .Where(a => a.RestauranteId == restauranteId && a.Plato != null)
                .Select(a => new DOM.RendimientoPlato
                {
                    PlatoId = a.Id,
                    Nombre = a.Nombre,
                    UnidadesVendidas = _ctx.ArticuloComanda
                        .Where(ac => ac.ArticuloId == a.Id 
                                  && ac.Comanda.RestauranteId == restauranteId
                                  && ac.Comanda.HoraInicio >= fechaDesde
                                  && ac.Comanda.HoraInicio <= fechaHasta
                                  && ac.Comanda.Pagos.Any())
                        .Sum(ac => (int?)ac.Cantidad) ?? 0,
                    FacturacionTotal = _ctx.ArticuloComanda
                        .Where(ac => ac.ArticuloId == a.Id 
                                  && ac.Comanda.RestauranteId == restauranteId
                                  && ac.Comanda.HoraInicio >= fechaDesde
                                  && ac.Comanda.HoraInicio <= fechaHasta
                                  && ac.Comanda.Pagos.Any())
                        .Sum(ac => (decimal?)(ac.Cantidad * a.PrecioVentaFinal)) ?? 0m
                });
        }

        public async Task<List<DOM.RendimientoPlato>> ObtenerTopPlatosMasVendidosAsync(int restauranteId, DateTime fechaDesde, DateTime fechaHasta, int limite = 5)
        {
            return await BaseQueryRendimiento(restauranteId, fechaDesde, fechaHasta)
                .OrderByDescending(p => p.UnidadesVendidas)
                .Take(limite)
                .ToListAsync();
        }

        public async Task<List<DOM.RendimientoPlato>> ObtenerTopPlatosMenosVendidosAsync(int restauranteId, DateTime fechaDesde, DateTime fechaHasta, int limite = 5)
        {
            return await BaseQueryRendimiento(restauranteId, fechaDesde, fechaHasta)
                .OrderBy(p => p.UnidadesVendidas)
                .Take(limite)
                .ToListAsync();
        }

        public async Task<DOM.TotalesPeriodo> ObtenerTotalesPeriodoAsync(int restauranteId, DateTime desde, DateTime hasta)
        {
            var query = _ctx.Comanda
                .Where(c => c.RestauranteId == restauranteId
                         && c.HoraInicio >= desde
                         && c.HoraInicio <= hasta
                         && c.Pagos.Any());

            var result = await query
                .GroupBy(c => 1)
                .Select(g => new DOM.TotalesPeriodo
                {
                    CantidadPedidos = g.Count(),
                    TotalFacturado = g.Sum(c => c.Pagos.Sum(p => p.Total))
                })
                .FirstOrDefaultAsync();

            return result ?? new DOM.TotalesPeriodo { CantidadPedidos = 0, TotalFacturado = 0 };
        }

        public async Task<List<DOM.VentaAgrupada>> ObtenerVentasAgrupadasAsync(int restauranteId, DateTime desde, DateTime hasta, TipoAgrupacionTiempo tipoAgrupacion)
        {
            var query = _ctx.Comanda
                .Where(c => c.RestauranteId == restauranteId
                         && c.HoraInicio >= desde
                         && c.HoraInicio <= hasta
                         && c.Pagos.Any());

            if (tipoAgrupacion == TipoAgrupacionTiempo.Hora)
            {
                var resultDb = await query
                    .GroupBy(c => c.HoraInicio.Hour)
                    .Select(g => new
                    {
                        Key = g.Key,
                        Total = g.Sum(c => c.Pagos.Sum(p => p.Total))
                    })
                    .OrderBy(x => x.Key)
                    .ToListAsync();

                return resultDb.Select(x => new DOM.VentaAgrupada
                {
                    Etiqueta = x.Key + "h",
                    Total = x.Total
                }).ToList();
            }
            else if (tipoAgrupacion == TipoAgrupacionTiempo.Dia)
            {
                var resultDb = await query
                    .GroupBy(c => c.HoraInicio.Date)
                    .Select(g => new
                    {
                        Key = g.Key,
                        Total = g.Sum(c => c.Pagos.Sum(p => p.Total))
                    })
                    .OrderBy(x => x.Key)
                    .ToListAsync();

                return resultDb.Select(x => new DOM.VentaAgrupada
                {
                    Etiqueta = x.Key.ToString("yyyy-MM-dd"),
                    Total = x.Total
                }).ToList();
            }
            else // Mes
            {
                var resultDb = await query
                    .GroupBy(c => new { c.HoraInicio.Year, c.HoraInicio.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Total = g.Sum(c => c.Pagos.Sum(p => p.Total))
                    })
                    .OrderBy(x => x.Year).ThenBy(x => x.Month)
                    .ToListAsync();

                return resultDb.Select(x => new DOM.VentaAgrupada
                {
                    Etiqueta = x.Year + "-" + x.Month.ToString("D2"),
                    Total = x.Total
                }).ToList();
            }
        }

        public async Task<List<DOM.EstadisticaMozo>> ObtenerEstadisticasMozosAsync(int restauranteId, DateTime desde, DateTime hasta)
        {
            var mozos = await _ctx.Mozos
                .Include(m => m.IdEmpleadoNavigation)
                .Where(m => m.IdEmpleadoNavigation.RestauranteId == restauranteId)
                .ToListAsync();

            var statsList = new List<DOM.EstadisticaMozo>();

            foreach (var mozo in mozos)
            {
                var comandasDelMozo = await _ctx.Comanda
                    .Include(c => c.Pagos)
                    .Where(c => c.RestauranteId == restauranteId
                             && c.HoraInicio >= desde
                             && c.HoraInicio <= hasta
                             && _ctx.Mozos
                                 .Where(m => m.IdEmpleado == mozo.IdEmpleado)
                                 .SelectMany(m => m.Mesas)
                                 .Any(mesa => mesa.Id == c.MesaId))
                    .ToListAsync();

                int mesasAtendidas = comandasDelMozo.Select(c => c.MesaId).Distinct().Count();
                decimal facturacionTotal = comandasDelMozo.Sum(c => c.Pagos.Sum(p => p.Total));

                var comandasFinalizadas = comandasDelMozo.Where(c => c.HoraFin.HasValue).ToList();
                string tiempoPromedioAtencion = "30m";
                if (comandasFinalizadas.Any())
                {
                    double avgMinutes = comandasFinalizadas.Average(c => (c.HoraFin.Value - c.HoraInicio).TotalMinutes);
                    tiempoPromedioAtencion = $"{Math.Round(avgMinutes)}m";
                }

                int activas = comandasDelMozo.Count(c => c.EstadoComandaId != (int)EstadoComanda.Finalizada 
                                                      && c.EstadoComandaId != (int)EstadoComanda.Abierta);
                string estado = "Baja carga";
                if (activas > 4)
                {
                    estado = "Sobrecargado";
                }
                else if (activas >= 2)
                {
                    estado = "Optimo";
                }

                statsList.Add(new DOM.EstadisticaMozo
                {
                    Nombre = mozo.IdEmpleadoNavigation.Nombre,
                    MesasAtendidas = mesasAtendidas,
                    FacturacionTotal = facturacionTotal,
                    TiempoPromedioAtencion = tiempoPromedioAtencion,
                    Estado = estado
                });
            }

            return statsList;
        }
    }

}




