using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EF = PanComido.Infraestructura.Persistencia.Entidades;
using PanComido.Infraestructura.Persistencia.Mappers;
namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class LoteRepositorio : ILoteRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly LoteEntityMapper _loteEntityMapper;

        public LoteRepositorio(AppDbContext ctx, LoteEntityMapper loteEntityMapper)
        {
            _ctx = ctx;
            _loteEntityMapper = loteEntityMapper;
        }

        public async Task<DateOnly?> ObtenerFechaDeVencimientoMasProximaDeInsumo(int insumoId)
        {
            return await _ctx.Lotes.Where(l => l.InsumoId == insumoId && !l.Eliminado)
                .OrderBy(l => l.FechaVencimiento)
                .Select(l => (DateOnly?)l.FechaVencimiento)
                .FirstOrDefaultAsync();
        }

        public async Task<decimal> ObtenerStockTotalDeInsumo(int insumoId)
        {
            return await _ctx.Lotes.Where(l => l.InsumoId == insumoId && !l.Eliminado)
                .SumAsync(l => (decimal?)l.Cantidad) ?? 0m;
        }

        public Task<Dictionary<(int insumoId, int bodegaId), DateOnly?>> ObtenerVencimientosPorBodega(int restauranteId)
        {
            return _ctx.Lotes
                .Where(l => l.Bodega.RestauranteId == restauranteId && !l.Eliminado)
                .GroupBy(l => new { l.InsumoId, l.BodegaId })
                .Select(g => new
                {
                    InsumoId = g.Key.InsumoId,
                    BodegaId = g.Key.BodegaId,
                    FechaVencimientoMasProxima = g.Min(l => (DateOnly?)l.FechaVencimiento)
                })
                .ToDictionaryAsync(x => (x.InsumoId, x.BodegaId), x => x.FechaVencimientoMasProxima);
        }

        public Task<Dictionary<(int insumoId, int bodegaId), decimal>> ObtenerStocksPorBodega(int restauranteId)
        {
            return _ctx.Lotes
                .Where(l => l.Bodega.RestauranteId == restauranteId && !l.Eliminado)
                .GroupBy(l => new { l.InsumoId, l.BodegaId })
                .Select(g => new
                {
                    InsumoId = g.Key.InsumoId,
                    BodegaId = g.Key.BodegaId,
                    StockTotal = g.Sum(l => (decimal?)l.Cantidad) ?? 0m
                })
                .ToDictionaryAsync(x => (x.InsumoId, x.BodegaId), x => x.StockTotal);
        }

        public async Task CrearLotesAsync(List<DOM.Lote> lotes)
        {
            var efLote = lotes.Select(_loteEntityMapper.paraEntidad).ToList();
            _ctx.Lotes.AddRange(efLote);
            await _ctx.SaveChangesAsync();
        }

        public async Task<int> ContarLotesConNombreBaseAsync(string nombreBase)
        {
            return await _ctx.Lotes
                .Where(l => l.Nombre.StartsWith(nombreBase) && !l.Eliminado)
                .CountAsync();
        }

        public async Task<Dictionary<int, decimal>> ObtenerStockTotalDeInsumosDisponible(int restauranteId, DateOnly fechaLimite)
        {
            var diccionarioStock = await _ctx.Lotes
                .AsNoTracking()
                .Where(l => l.Insumo.IdArticuloNavigation.RestauranteId == restauranteId && !l.Eliminado)
                .Where(l => l.FechaVencimiento == null || l.FechaVencimiento >= fechaLimite)
                .GroupBy(l => l.InsumoId)
                .Select(grupo => new
                {
                    InsumoId = grupo.Key,
                    TotalStock = grupo.Sum(l => l.Cantidad)
                })
                .ToDictionaryAsync(k => k.InsumoId, v => v.TotalStock);

            return diccionarioStock;
        }

        public async Task<List<DOM.Lote>> ObtenerLotesPorRestauranteAsync(int restauranteId)
        {
            List<EF.Lote> efLotes = await _ctx.Lotes
                .AsNoTracking()
                .Where(l => l.Bodega.RestauranteId == restauranteId && l.Cantidad > 0 && !l.Eliminado)
                .OrderBy(l => l.FechaVencimiento)
                .ThenBy(l => l.Nombre)
                .ToListAsync();

            return efLotes.Select(l => _loteEntityMapper.paraDominio(l)).ToList();
        }

        public async Task<DOM.Lote> ObtenerPorIdAsync(int restauranteId, int loteId)
        {
            var efLote = await _ctx.Lotes
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == loteId && l.Bodega.RestauranteId == restauranteId && !l.Eliminado);

            return efLote != null ? _loteEntityMapper.paraDominio(efLote) : null;
        }

        public async Task<List<DOM.Lote>> ObtenerLotesPorFechaVencimientoAscendenteAsync(int restauranteId, int insumoId)
        {
            List<EF.Lote> efLotes = await _ctx.Lotes
                            .AsNoTracking()
                            .Where(l => l.Bodega.RestauranteId == restauranteId
                                        && l.InsumoId == insumoId
                                        && l.Cantidad > 0
                                        && !l.Eliminado)
                            .OrderBy(l => l.FechaVencimiento)
                            .ToListAsync();

            return efLotes.Select(l => _loteEntityMapper.paraDominio(l)).ToList();
        }

        public async Task ActualizarLotesAsync(List<DOM.Lote> lotesModificados)
        {
            List<EF.Lote> efLotes = lotesModificados.Select(l => _loteEntityMapper.paraEntidad(l)).ToList();

            _ctx.Lotes.UpdateRange(efLotes);
            await _ctx.SaveChangesAsync();
        }

        public async Task<bool> EliminarAsync(int restauranteId, int loteId)
        {
            var lote = await _ctx.Lotes.FirstOrDefaultAsync(l => l.Id == loteId && l.Bodega.RestauranteId == restauranteId && !l.Eliminado);
            if (lote == null) return false;

            lote.Eliminado = true;
            _ctx.Lotes.Update(lote);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}
