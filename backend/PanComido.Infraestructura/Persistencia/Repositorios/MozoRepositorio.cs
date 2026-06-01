using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class MozoRepositorio : IMozoRepositorio
    {
        private readonly AppDbContext _ctx;

        public MozoRepositorio(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<int> ObtenerMozoAsignadoAMesaAsync(int mesaId)
        {
            return await _ctx.Mesas
                .Include(m => m.Mozos)
                .Where(m => m.Id == mesaId)
                .Select(m => m.Mozos.First().IdEmpleado)
                .FirstOrDefaultAsync();
        }
    }
}
