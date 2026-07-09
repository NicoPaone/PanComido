using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class BebidaPreparadaRepositorio : IBebidaPreparadaRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly ArticuloEntityMapper _articuloMapper;

        public BebidaPreparadaRepositorio(AppDbContext ctx, ArticuloEntityMapper articuloMapper)
        {
            _ctx = ctx;
            _articuloMapper = articuloMapper;
        }

        public async Task<DOM.BebidaPreparada> CrearAsync(DOM.BebidaPreparada bebidaPreparadaDominio)
        {
            var efArticulo = _articuloMapper.paraEntidad(bebidaPreparadaDominio);
            await ConfiguracionVisibilidadHelper.AplicarVisibilidadEnCartaAsync(_ctx, efArticulo, bebidaPreparadaDominio.EsVisibleEnCarta);

            await _ctx.Articulos.AddAsync(efArticulo);
            await _ctx.SaveChangesAsync();

            return await ObtenerPorIdAsync(efArticulo.Id, bebidaPreparadaDominio.RestauranteId);
        }

        public async Task<bool> ExisteBebidaPreparadaConNombreAsync(int restauranteId, string nombre)
        {
            return await _ctx.Articulos
                .AnyAsync(a => a.RestauranteId == restauranteId
                            && a.BebidaPreparadum != null
                            && !a.Eliminado
                            && a.Nombre.ToLower() == nombre.ToLower());
        }

        public async Task<DOM.BebidaPreparada> ObtenerPorIdAsync(int bebidaPreparadaId, int restauranteId)
        {
            var efArticulo = await _ctx.Articulos
                .Include(a => a.BebidaPreparadum)
                    .ThenInclude(bp => bp.BebidaPreparadaInsumos)
                        .ThenInclude(bpi => bpi.Insumo)
                            .ThenInclude(i => i.IdArticuloNavigation)
                .Include(a => a.BebidaPreparadum)
                    .ThenInclude(bp => bp.BebidaPreparadaInsumos)
                        .ThenInclude(bpi => bpi.Insumo)
                            .ThenInclude(i => i.CategoriaInsumo)
                .Include(a => a.ConfiguracionArticulos)
                .FirstOrDefaultAsync(a => a.Id == bebidaPreparadaId && a.RestauranteId == restauranteId && a.BebidaPreparadum != null);

            if (efArticulo == null) return null;

            return (DOM.BebidaPreparada)_articuloMapper.paraDominio(efArticulo);
        }

        public async Task<DOM.BebidaPreparada> ActualizarAsync(DOM.BebidaPreparada bebidaPreparadaDominio)
        {
            var efArticulo = await _ctx.Articulos
                .Include(a => a.BebidaPreparadum)
                    .ThenInclude(bp => bp.BebidaPreparadaInsumos)
                .Include(a => a.ConfiguracionArticulos)
                .FirstOrDefaultAsync(a => a.Id == bebidaPreparadaDominio.Id && a.RestauranteId == bebidaPreparadaDominio.RestauranteId && a.BebidaPreparadum != null);

            if (efArticulo == null) return null;

            ActualizarDatosBasicos(efArticulo, bebidaPreparadaDominio);
            ActualizarInsumos(efArticulo, bebidaPreparadaDominio.Insumos);
            await ConfiguracionVisibilidadHelper.AplicarVisibilidadEnCartaAsync(_ctx, efArticulo, bebidaPreparadaDominio.EsVisibleEnCarta);

            await _ctx.SaveChangesAsync();
            return await ObtenerPorIdAsync(efArticulo.Id, efArticulo.RestauranteId);
        }

        public async Task<DOM.BebidaPreparada> EliminarAsync(int bebidaPreparadaId, int restauranteId)
        {
            var efArticulo = await _ctx.Articulos
                .FirstOrDefaultAsync(a => a.Id == bebidaPreparadaId && a.RestauranteId == restauranteId && a.BebidaPreparadum != null);

            if (efArticulo == null) return null;

            efArticulo.Eliminado = true;
            await _ctx.SaveChangesAsync();

            return new DOM.BebidaPreparada { Id = efArticulo.Id, Nombre = efArticulo.Nombre };
        }
        private void ActualizarDatosBasicos(EF.Articulo efArticulo, DOM.BebidaPreparada bebidaPreparadaDominio)
        {
            efArticulo.Nombre = bebidaPreparadaDominio.Nombre;
            efArticulo.Descripcion = bebidaPreparadaDominio.Descripcion;
            efArticulo.PrecioVentaFinal = bebidaPreparadaDominio.PrecioVentaFinal;
            efArticulo.UrlImagen = bebidaPreparadaDominio.UrlImagen;
        }

        private void ActualizarInsumos(EF.Articulo efArticulo, List<DOM.BebidaPreparadaInsumo> insumosDominio)
        {
            var insumosNuevos = insumosDominio.Select(i => i.InsumoId).ToList();
            var entidadesAEliminar = efArticulo.BebidaPreparadum.BebidaPreparadaInsumos
                .Where(bpi => !insumosNuevos.Contains(bpi.InsumoId))
                .ToList();

            foreach (var aEliminar in entidadesAEliminar)
            {
                _ctx.Set<EF.BebidaPreparadaInsumo>().Remove(aEliminar);
            }

            foreach (var insDominio in insumosDominio)
            {
                var existente = efArticulo.BebidaPreparadum.BebidaPreparadaInsumos.FirstOrDefault(bpi => bpi.InsumoId == insDominio.InsumoId);
                if (existente != null)
                {
                    existente.Cantidad = insDominio.Cantidad;
                }
                else
                {
                    efArticulo.BebidaPreparadum.BebidaPreparadaInsumos.Add(new EF.BebidaPreparadaInsumo
                    {
                        InsumoId = insDominio.InsumoId,
                        Cantidad = insDominio.Cantidad
                    });
                }
            }
        }
    }
}