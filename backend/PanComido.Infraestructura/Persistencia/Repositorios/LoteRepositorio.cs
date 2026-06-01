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
            return await _ctx.Lotes.Where(l => l.InsumoId == insumoId)
                .OrderBy(l => l.FechaVencimiento)
                .Select(l => (DateOnly?)l.FechaVencimiento)
                .FirstOrDefaultAsync();
        }

        public async Task<decimal> ObtenerStockTotalDeInsumo(int insumoId)
        {
            return await _ctx.Lotes.Where(l => l.InsumoId == insumoId)
                .SumAsync(l => (decimal?)l.Cantidad) ?? 0m;
        }

        public Task<Dictionary<(int insumoId, int bodegaId), DateOnly?>> ObtenerVencimientosPorBodega(int restauranteId)
        {
            return _ctx.Lotes
                .Where(l => l.Bodega.RestauranteId == restauranteId)
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
                .Where(l => l.Bodega.RestauranteId == restauranteId)
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
                .Where(l => l.Nombre.StartsWith(nombreBase))
                .CountAsync();
        }

        public Task<Dictionary<int, decimal>> ObtenerStockTotalDeInsumos(int restauranteId)
        {
            throw new NotImplementedException();
        }
    }
}
