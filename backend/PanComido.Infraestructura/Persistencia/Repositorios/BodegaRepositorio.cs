using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class BodegaRepositorio : IBodegaRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly BodegaEntityMapper _mapper;
        public BodegaRepositorio(AppDbContext ctx, BodegaEntityMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }

        public async Task<bool> ExisteBodegaEnRestauranteAsync(int restauranteId, int bodegaId)
        {
            return await _ctx.Bodegas.AnyAsync(b => b.Id == bodegaId && b.RestauranteId == restauranteId);
        }

        public async Task<List<DOM.Bodega>> ObtenerBodegasAsync(int restauranteId)
        {
            List<EF.Bodega> bodegas = await _ctx.Bodegas
                .Include(b => b.TipoBodega) 
                .Where(b => b.RestauranteId == restauranteId && !b.Eliminado)
                .ToListAsync();

            return bodegas.Select(b => _mapper.paraDominio(b)).ToList();
        }

        public async Task<DOM.Bodega> ObtenerBodegaPorIdAsync(int id, int restauranteId)
        {
            var bodegaEF = await _ctx.Bodegas
                .Include(b => b.TipoBodega)
                .FirstOrDefaultAsync(b => b.Id == id && b.RestauranteId == restauranteId && !b.Eliminado);

            return _mapper.paraDominio(bodegaEF);
        }

        public async Task<DOM.Bodega> CrearAsync(DOM.Bodega bodega, int restauranteId)
        {
            var bodegaEF = _mapper.paraEntity(bodega, restauranteId);
            _ctx.Bodegas.Add(bodegaEF);
            await _ctx.SaveChangesAsync();

            return await ObtenerBodegaPorIdAsync(bodegaEF.Id, restauranteId);
        }

        public async Task<DOM.Bodega> ModificarAsync(DOM.Bodega bodega, int restauranteId)
        {
            var bodegaEF = await _ctx.Bodegas
                .FirstOrDefaultAsync(b => b.Id == bodega.Id && b.RestauranteId == restauranteId && !b.Eliminado);

            if (bodegaEF != null)
            {
                bodegaEF.Nombre = bodega.Nombre;
                bodegaEF.TipoBodegaId = bodega.TipoBodegaId;

                _ctx.Bodegas.Update(bodegaEF);
                await _ctx.SaveChangesAsync();
            }

            return await ObtenerBodegaPorIdAsync(bodega.Id, restauranteId);
        }

        public async Task<bool> EliminarAsync(int id, int restauranteId)
        {
            var bodegaEF = await _ctx.Bodegas
                .FirstOrDefaultAsync(b => b.Id == id && b.RestauranteId == restauranteId && !b.Eliminado);

            if (bodegaEF == null) return false;

            bodegaEF.Eliminado = true;
            _ctx.Bodegas.Update(bodegaEF);
            await _ctx.SaveChangesAsync();

            return true;
        }

        public async Task<bool> TieneLotesAsociadosAsync(int bodegaId)
        {
            return await _ctx.Lotes
                .AnyAsync(l => l.BodegaId == bodegaId && !l.Eliminado && !l.Insumo.IdArticuloNavigation.Eliminado);
        }

        public async Task<bool> ExisteBodegaPorNombreAsync(string nombre, int restauranteId, int? idExcluido = null)
        {
            var query = _ctx.Bodegas.Where(b => b.Nombre.ToLower() == nombre.ToLower() && b.RestauranteId == restauranteId && !b.Eliminado);
            
            if (idExcluido.HasValue)
            {
                query = query.Where(b => b.Id != idExcluido.Value);
            }

            return await query.AnyAsync();
        }


    }
}
