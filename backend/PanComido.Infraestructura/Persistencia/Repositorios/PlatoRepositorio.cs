using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class PlatoRepositorio : IPlatoRepositorio

    {
        private readonly AppDbContext _ctx;
        private readonly ArticuloEntityMapper _articuloMapper;


        public PlatoRepositorio(AppDbContext ctx, ArticuloEntityMapper articuloMapper)
        {
            _ctx = ctx;
            _articuloMapper = articuloMapper;
        }


        public async Task CrearAsync(Dominio.Entidades.Plato platoDominio)
        {
            var efArticulo = _articuloMapper.paraEntidad(platoDominio);

            // 2. Relación Muchos-a-Muchos: Restricciones
            // Entity Framework necesita que busquemos las restricciones existentes en la BD
            // para "atarlas" al plato nuevo, en lugar de intentar crear restricciones nuevas.
            if (platoDominio.Restricciones != null && platoDominio.Restricciones.Any())
            {
                var idsRestricciones = platoDominio.Restricciones.Select(r => r.Id).ToList();
                var restriccionesDb = await _ctx.Restriccions
                                                .Where(r => idsRestricciones.Contains(r.Id))
                                                .ToListAsync();

                efArticulo.Plato.Restriccions = restriccionesDb;
            }


            await _ctx.Articulos.AddAsync(efArticulo);
            await _ctx.SaveChangesAsync();


        }

        public async Task<bool> ExistePlatoConNombreAsync(int restauranteId, string nombre)
        {
            return await _ctx.Articulos
                .AnyAsync(a => a.RestauranteId == restauranteId 
                            && a.Plato != null 
                            && !a.Eliminado 
                            && a.Nombre.ToLower() == nombre.ToLower());
        }

        public async Task<Dominio.Entidades.Plato> ObtenerPorIdAsync(int platoId, int restauranteId)
        {
            var efArticulo = await _ctx.Articulos
                .Include(a => a.Plato)
                    .ThenInclude(p => p.Restriccions)
                .Include(a => a.Plato)
                    .ThenInclude(p => p.PlatoIngredientes)
                        .ThenInclude(pi => pi.Ingrediente)
                            .ThenInclude(i => i.IdInsumoNavigation)
                                .ThenInclude(i => i.IdArticuloNavigation)
                .Include(a => a.ConfiguracionArticulos)
                .FirstOrDefaultAsync(a => a.Id == platoId && a.RestauranteId == restauranteId && a.Plato != null);

            if (efArticulo == null)
            {
                return null;
            }

            return (Dominio.Entidades.Plato)_articuloMapper.paraDominio(efArticulo);
        }

        public async Task ActualizarAsync(Dominio.Entidades.Plato platoDominio)
        {
            var efArticulo = await _ctx.Articulos
                .Include(a => a.Plato)
                    .ThenInclude(p => p.Restriccions)
                .Include(a => a.Plato)
                    .ThenInclude(p => p.PlatoIngredientes)
                .Include(a => a.ConfiguracionArticulos)
                .FirstOrDefaultAsync(a => a.Id == platoDominio.Id && a.RestauranteId == platoDominio.RestauranteId && a.Plato != null);

            if (efArticulo == null)
            {
                throw new InvalidOperationException("Plato no encontrado para actualizar.");
            }

            // Actualizar datos de Articulo
            efArticulo.Nombre = platoDominio.Nombre;
            efArticulo.Descripcion = platoDominio.Descripcion;
            efArticulo.PrecioVentaFinal = platoDominio.PrecioVentaFinal;
            efArticulo.UrlImagen = platoDominio.UrlImagen;

            var configVisible = await _ctx.ConfiguracionArticulos.FindAsync(2);
            if (platoDominio.EsVisibleEnCarta)
            {
                if (!efArticulo.ConfiguracionArticulos.Any(c => c.Id == 2) && configVisible != null)
                {
                    efArticulo.ConfiguracionArticulos.Add(configVisible);
                }
            }
            else
            {
                var cfg = efArticulo.ConfiguracionArticulos.FirstOrDefault(c => c.Id == 2);
                if (cfg != null)
                {
                    efArticulo.ConfiguracionArticulos.Remove(cfg);
                }
            }

            // Actualizar datos de Plato
            efArticulo.Plato.TiempoPreparacionBase = platoDominio.TiempoPreparacionBase;
            efArticulo.Plato.TipoPlatoId = platoDominio.TipoPlatoId;
            efArticulo.Plato.CategoriaPlatoId = platoDominio.CategoriaPlatoId;

            // Actualizar Restricciones
            efArticulo.Plato.Restriccions.Clear();
            if (platoDominio.Restricciones != null && platoDominio.Restricciones.Any())
            {
                var idsRestricciones = platoDominio.Restricciones.Select(r => r.Id).ToList();
                var restriccionesDb = await _ctx.Restriccions
                                                .Where(r => idsRestricciones.Contains(r.Id))
                                                .ToListAsync();
                foreach (var rest in restriccionesDb)
                {
                    efArticulo.Plato.Restriccions.Add(rest);
                }
            }

            // Actualizar Ingredientes
            var ingredientesActuales = efArticulo.Plato.PlatoIngredientes.ToList();
            var idsNuevos = platoDominio.Ingredientes.Select(i => i.InsumoId).ToList();

            foreach (var existente in ingredientesActuales)
            {
                if (!idsNuevos.Contains(existente.IngredienteId))
                {
                    _ctx.Set<Entidades.PlatoIngrediente>().Remove(existente);
                }
            }

            foreach (var ingDominio in platoDominio.Ingredientes)
            {
                var existente = efArticulo.Plato.PlatoIngredientes.FirstOrDefault(pi => pi.IngredienteId == ingDominio.InsumoId);
                if (existente != null)
                {
                    existente.Cantidad = ingDominio.Cantidad;
                    existente.Opcional = ingDominio.Opcional;
                }
                else
                {
                    efArticulo.Plato.PlatoIngredientes.Add(new Entidades.PlatoIngrediente
                    {
                        IngredienteId = ingDominio.InsumoId,
                        Cantidad = ingDominio.Cantidad,
                        Opcional = ingDominio.Opcional
                    });
                }
            }

            await _ctx.SaveChangesAsync();
        }
    }

    }
