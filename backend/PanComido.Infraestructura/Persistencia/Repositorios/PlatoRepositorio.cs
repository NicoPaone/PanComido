using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

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


        public async Task CrearAsync(DOM.Plato platoDominio)
        {
            var efArticulo = _articuloMapper.paraEntidad(platoDominio);

            efArticulo.Plato.Restriccions = await ObtenerRestriccionesDbAsync(platoDominio.Restricciones);

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

        public async Task<DOM.Plato> ObtenerPorIdAsync(int platoId, int restauranteId)
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

            return (DOM.Plato)_articuloMapper.paraDominio(efArticulo);
        }

        public async Task ActualizarAsync(DOM.Plato platoDominio)
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

            ActualizarDatosBasicos(efArticulo, platoDominio);
            await ActualizarConfiguracionVisibilidadAsync(efArticulo, platoDominio.EsVisibleEnCarta);
            await ActualizarRestriccionesAsync(efArticulo, platoDominio.Restricciones);
            ActualizarIngredientes(efArticulo, platoDominio.Ingredientes);

            await _ctx.SaveChangesAsync();
        }

        public async Task EliminarAsync(int platoId, int restauranteId)
        {
            var efArticulo = await _ctx.Articulos
                .FirstOrDefaultAsync(a => a.Id == platoId && a.RestauranteId == restauranteId && a.Plato != null);

            if (efArticulo == null)
            {
                throw new KeyNotFoundException("El plato no existe o no pertenece al restaurante.");
            }

            efArticulo.Eliminado = true;
            await _ctx.SaveChangesAsync();
        }

        private async Task<List<EF.Restriccion>> ObtenerRestriccionesDbAsync(List<DOM.Restriccion> restriccionesDominio)
        {
            if (restriccionesDominio == null || !restriccionesDominio.Any())
                return new List<EF.Restriccion>();

            var ids = restriccionesDominio.Select(r => r.Id).ToList();
            return await _ctx.Restriccions.Where(r => ids.Contains(r.Id)).ToListAsync();
        }

        private void ActualizarDatosBasicos(EF.Articulo efArticulo, DOM.Plato platoDominio)
        {
            efArticulo.Nombre = platoDominio.Nombre;
            efArticulo.Descripcion = platoDominio.Descripcion;
            efArticulo.PrecioVentaFinal = platoDominio.PrecioVentaFinal;
            efArticulo.UrlImagen = platoDominio.UrlImagen;

            efArticulo.Plato.TiempoPreparacionBase = platoDominio.TiempoPreparacionBase;
            efArticulo.Plato.TipoPlatoId = platoDominio.TipoPlatoId;
            efArticulo.Plato.CategoriaPlatoId = platoDominio.CategoriaPlatoId;
        }

        private Task ActualizarConfiguracionVisibilidadAsync(EF.Articulo efArticulo, bool esVisibleEnCarta)
        {
            return ConfiguracionVisibilidadHelper.AplicarVisibilidadEnCartaAsync(_ctx, efArticulo, esVisibleEnCarta);
        }

        private async Task ActualizarRestriccionesAsync(EF.Articulo efArticulo, List<DOM.Restriccion> restriccionesDominio)
        {
            efArticulo.Plato.Restriccions.Clear();
            var restriccionesDb = await ObtenerRestriccionesDbAsync(restriccionesDominio);
            foreach (var rest in restriccionesDb)
            {
                efArticulo.Plato.Restriccions.Add(rest);
            }
        }

        private void ActualizarIngredientes(EF.Articulo efArticulo, List<DOM.PlatoIngrediente> ingredientesDominio)
        {
            var insumosNuevos = ingredientesDominio.Select(i => i.InsumoId).ToList();
            var entidadesAEliminar = efArticulo.Plato.PlatoIngredientes
                .Where(pi => !insumosNuevos.Contains(pi.IngredienteId))
                .ToList();

            foreach (var aEliminar in entidadesAEliminar)
            {
                _ctx.Set<EF.PlatoIngrediente>().Remove(aEliminar);
            }

            foreach (var ingDominio in ingredientesDominio)
            {
                var existente = efArticulo.Plato.PlatoIngredientes.FirstOrDefault(pi => pi.IngredienteId == ingDominio.InsumoId);
                if (existente != null)
                {
                    existente.Cantidad = ingDominio.Cantidad;
                    existente.Opcional = ingDominio.Opcional;
                }
                else
                {
                    efArticulo.Plato.PlatoIngredientes.Add(new EF.PlatoIngrediente
                    {
                        IngredienteId = ingDominio.InsumoId,
                        Cantidad = ingDominio.Cantidad,
                        Opcional = ingDominio.Opcional
                    });
                }
            }
        }



    }

}
