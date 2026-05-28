using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class LoteRepositorio : ILoteRepositorio
    {
        private readonly AppDbContext _ctx;
        public LoteRepositorio(AppDbContext ctx)
        {
            _ctx = ctx;
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

        

        
    }
}
