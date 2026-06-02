using Microsoft.EntityFrameworkCore;
using DOM = PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using EF = PanComido.Infraestructura.Persistencia.Entidades;
using PanComido.Infraestructura.Persistencia.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class LlamadoRepositorio : ILlamadoRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly LlamadoEntityMapper _llamadoMapper;

        public LlamadoRepositorio(AppDbContext ctx, LlamadoEntityMapper llamadoMapper)
        {
            _ctx = ctx;
            _llamadoMapper = llamadoMapper;
        }

        public async Task crearLlamadoAsync(DOM.Llamado llamado)
        {
            EF.Llamado efLlamado = _llamadoMapper.paraEntidad(llamado);
            await _ctx.Llamados.AddAsync(efLlamado);

            await _ctx.SaveChangesAsync();
        }

        public async Task<List<DOM.Llamado>> ObtenerLlamadosPendientesPorMozoAsync(int mozoId)
        {
            return await _ctx.Llamados
                .Include(l => l.CategoriaLlamado)
                .Include(l => l.Mozo).ThenInclude(m => m.Mesas)
                .Where(l => l.MozoId == mozoId && !l.Resuelto)
                .Select(l => _llamadoMapper.paraDominio(l))
                .ToListAsync();
        }

        public async Task<bool> ResolverLlamadoAsync(int llamadoId)
        {
            var efLlamado = await _ctx.Llamados.FirstOrDefaultAsync(l => l.Id == llamadoId);
            if (efLlamado == null)  return false;
            efLlamado.Resuelto = true;
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}
