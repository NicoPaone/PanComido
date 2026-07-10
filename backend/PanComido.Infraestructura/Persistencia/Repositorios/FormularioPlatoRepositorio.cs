using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public  class FormularioPlatoRepositorio : IFormularioPlatoRepositorio

    {
        private readonly AppDbContext _ctx;
        private readonly FormularioParaCrearPlatoEntityMapper _mapper;
        private readonly IngredientePreparadoEntityMapper _ingredientePreparadoMapper;

       public FormularioPlatoRepositorio(AppDbContext ctx, FormularioParaCrearPlatoEntityMapper mapper, IngredientePreparadoEntityMapper ingredientePreparadoMapper)
        {
            _ctx = ctx;
            _mapper = mapper;
            _ingredientePreparadoMapper = ingredientePreparadoMapper;
        }

        public async Task<List<CategoriaPlato>> ObtenerCategoriasPlatoAsync()
        {

          var lista = await _ctx.CategoriaPlatos.ToListAsync();
            return lista.Select(x => _mapper.paraDominio(x)).ToList();

        }

        public async Task<List<Ingrediente>> ObtenerIngredientesBaseAsync(int restauranteId)
        {
            var lista = await _ctx.Ingredientes
                .Include(i => i.IdInsumoNavigation)
                    .ThenInclude(ins => ins.IdArticuloNavigation)

                .Include(i => i.IdInsumoNavigation)
                    .ThenInclude(ins => ins.UnidadMedida)

                .Include(i => i.IdInsumoNavigation)
                    .ThenInclude(ins => ins.PedidoInsumos)
                        .ThenInclude(pi => pi.Pedido)

                .Where(i => i.IdInsumoNavigation.IdArticuloNavigation.RestauranteId == restauranteId && !i.IdInsumoNavigation.IdArticuloNavigation.Eliminado)
                .ToListAsync();

            return lista.Select(x => _mapper.paraDominio(x)).ToList();
        }

        public async Task<List<IngredientePreparado>> ObtenerIngredientesPreparadosAsync(int restauranteId)
        {
            var lista = await _ctx.IngredientePreparados
                .Include(ip => ip.IdIngredienteNavigation)
                    .ThenInclude(ing => ing.IdInsumoNavigation)
                        .ThenInclude(ins => ins.IdArticuloNavigation)
                .Include(ip => ip.IdIngredienteNavigation)
                    .ThenInclude(ing => ing.IdInsumoNavigation)
                        .ThenInclude(ins => ins.UnidadMedida)
                .Where(ip => ip.IdIngredienteNavigation.IdInsumoNavigation.IdArticuloNavigation.RestauranteId == restauranteId && !ip.IdIngredienteNavigation.IdInsumoNavigation.IdArticuloNavigation.Eliminado)
                .ToListAsync();

            return lista.Select(x => _ingredientePreparadoMapper.ParaDominio(x)).ToList();
        }

        public async Task<List<Restriccion>> ObtenerRestriccionesAsync()
        {
            var lista = await _ctx.Restriccions.ToListAsync();
            return lista.Select(x => _mapper.paraDominio(x)).ToList();
        }

        public async Task<List<TipoPlato>> ObtenerTiposPlatoAsync()
        {
            var lista = await _ctx.TipoPlatos.ToListAsync();
            return lista.Select(x => _mapper.paraDominio(x)).ToList();
        }
    }
}
