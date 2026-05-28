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

        public async Task<DateOnly> ObtenerFechaDeVencimientoMasProximaDeInsumo(int insumoId)
        {
            return await _ctx.Lotes.Where(l => l.InsumoId == insumoId)
                .OrderBy(l => l.FechaVencimiento)
                .Select(l => (DateOnly?)l.FechaVencimiento)
                .FirstOrDefaultAsync() ?? DateOnly.MaxValue;
        }

        public async Task<decimal> ObtenerStockTotalDeInsumo(int insumoId)
        {
            return await _ctx.Lotes.Where(l => l.InsumoId == insumoId)
                .SumAsync(l => (decimal?)l.Cantidad) ?? 0m;
        }
    }
}
