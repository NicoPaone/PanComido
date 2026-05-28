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
        public async Task<List<DOM.Bodega>> ObtenerBodegasAsync(int restauranteId)
        {
            List<EF.Bodega> bodegas = await _ctx.Bodegas
                .Where(b => b.RestauranteId == restauranteId)
                .ToListAsync();

            return bodegas.Select(b => _mapper.paraDominio(b)).ToList();
        }
    }
}
