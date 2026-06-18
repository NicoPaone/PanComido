using Microsoft.EntityFrameworkCore;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class PorcentajesGananciaRepositorio : IPorcentajesCategoriaRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly PorcentajesCategoriaEntityMapper _porcentajesCategoriaEntityMapper;

        public PorcentajesGananciaRepositorio(AppDbContext ctx, PorcentajesCategoriaEntityMapper porcentajesCategoriaEntityMapper)
        {
            _ctx = ctx;
            _porcentajesCategoriaEntityMapper = porcentajesCategoriaEntityMapper;
        }

        public async Task<DOM.PorcentajesGanancia> ActualizarPorcentajesGananciaAsync(int restauranteId, List<DOM.PorcentajesCategoria> platos, List<DOM.PorcentajesCategoria> bebidas)
        {
            var efPlatos = await _ctx.PorcentajeCategoriaPlatos
                .Include(p => p.CategoriaPlato)
                .Where(p => p.RestauranteId == restauranteId)
                .ToListAsync();

            foreach (var plato in platos)
            {
                var efPlato = efPlatos.FirstOrDefault(p => p.CategoriaPlatoId == plato.Id);
                if (efPlato != null)
                    efPlato.Porcentaje = plato.Porcentaje;
            }

            var efBebidas = await _ctx.PorcentajeCategoriaBebida
                .Include(p => p.CategoriaInsumo)
                .Where(p => p.RestauranteId == restauranteId)
                .ToListAsync();

            foreach (var bebida in bebidas)
            {
                var efBebida = efBebidas.FirstOrDefault(p => p.CategoriaInsumoId == bebida.Id);
                if (efBebida != null)
                    efBebida.Porcentaje = bebida.Porcentaje;
            }

            await _ctx.SaveChangesAsync();
            return new DOM.PorcentajesGanancia
            {
                Platos = efPlatos.Select(_porcentajesCategoriaEntityMapper.paraDominio).ToList(),
                Bebidas = efBebidas.Select(_porcentajesCategoriaEntityMapper.paraDominio).ToList()
            };
        }

        public async Task<DOM.PorcentajesGanancia> ObtenerPorcentajesGananciaAsync(int restauranteId)
        {
            var efPlatos = await _ctx.PorcentajeCategoriaPlatos
                .Include(p => p.CategoriaPlato)
                .Where(p => p.RestauranteId == restauranteId)
                .ToListAsync();

            var efBebidas = await _ctx.PorcentajeCategoriaBebida
                .Include(b => b.CategoriaInsumo)
                .Where(b => b.RestauranteId == restauranteId)
                .ToListAsync();

            List<DOM.PorcentajesCategoria> platos = efPlatos.Select(_porcentajesCategoriaEntityMapper.paraDominio).ToList();
            List<DOM.PorcentajesCategoria> bebidas = efBebidas.Select(_porcentajesCategoriaEntityMapper.paraDominio).ToList();

            return new DOM.PorcentajesGanancia
            {
                Platos = platos,
                Bebidas = bebidas
            };

        }
    }
}
