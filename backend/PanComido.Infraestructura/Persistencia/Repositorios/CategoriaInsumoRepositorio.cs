using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Entidades;
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
    public class CategoriaInsumoRepositorio : ICategoriaInsumoRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly CategoriaInsumoEntityMapper _mapper;

        public CategoriaInsumoRepositorio(AppDbContext ctx, CategoriaInsumoEntityMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }


        public async Task<List<DOM.CategoriaInsumo>> ObtenerCategoriasInsumoAsync()
        {
            List<EF.CategoriaInsumo> categoriasEF = await _ctx.CategoriaInsumos.ToListAsync();
            return categoriasEF.Select(c => _mapper.paraDominio(c)).ToList();
        }

        public async Task<DOM.CategoriaInsumo> ObtenerPorIdAsync(int categoriaId)
        {
            EF.CategoriaInsumo categoriaBuscada = await _ctx.CategoriaInsumos
                .Where(c => c.Id == categoriaId)
                .FirstOrDefaultAsync();

            return _mapper.paraDominio(categoriaBuscada);
        }
    }
}
