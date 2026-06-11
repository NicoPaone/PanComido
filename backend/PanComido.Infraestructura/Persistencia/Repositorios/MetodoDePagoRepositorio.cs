using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class MetodoDePagoRepositorio : IMetodoDePagoRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly MetodoDePagoEntityMapper _metodoDePagoEntityMapper;

        public MetodoDePagoRepositorio(AppDbContext context, MetodoDePagoEntityMapper metodoDePagoMapper)
        {
            _ctx = context;
            _metodoDePagoEntityMapper = metodoDePagoMapper;
        }

        public async Task<List<MetodoDePago>> ObtenerMetodosDePagoAsync(int restauranteId)
        {
            var efMetodos = await _ctx.MetodoDePagos
                .Include(m => m.MetodoDePagoRestaurantes
                    .Where(r => r.RestauranteId == restauranteId))
                .ToListAsync();

            return efMetodos.Select(m => _metodoDePagoEntityMapper.paraDominio(m, m.MetodoDePagoRestaurantes.FirstOrDefault())).ToList();
        }

        public async Task ActualizarEstadoAsync(int restauranteId, List<MetodoDePago> metodosDePago)
        {
            foreach (var metodo in metodosDePago)
            {
                var efRelacion = await _ctx.MetodoDePagoRestaurantes
                    .FirstOrDefaultAsync(r => r.RestauranteId == restauranteId
                                            && r.MetodoDePagoId == metodo.Id);
                if (efRelacion != null)
                    efRelacion.Habilitado = metodo.Habilitado;
            }
            await _ctx.SaveChangesAsync();
        }
    }
}
