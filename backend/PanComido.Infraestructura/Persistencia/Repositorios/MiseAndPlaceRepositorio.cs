using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Entidades;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class MiseAndPlaceRepositorio : IMiseAndPlaceRepositorio
    {
        private readonly AppDbContext _ctx;

        public MiseAndPlaceRepositorio(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<int> CrearMiseAndPlaceAsync(NuevoMiseAndPlace nuevoMiseAndPlace, string nombreLote)
        {
            using var transaction = await _ctx.Database.BeginTransactionAsync();
            try
            {
                var articuloDb = new PanComido.Infraestructura.Persistencia.Entidades.Articulo
                {
                    Nombre = nuevoMiseAndPlace.Nombre,
                    Descripcion = nuevoMiseAndPlace.Descripcion,
                    PrecioVentaFinal = 0,
                    PrecioGanancia = 0,
                    PrecioPromocional = 0,
                    RestauranteId = nuevoMiseAndPlace.RestauranteId,
                    Eliminado = false
                };

                await _ctx.Articulos.AddAsync(articuloDb);
                await _ctx.SaveChangesAsync();

                var insumoDb = new PanComido.Infraestructura.Persistencia.Entidades.Insumo
                {
                    IdArticulo = articuloDb.Id,
                    CategoriaInsumoId = nuevoMiseAndPlace.CategoriaId,
                    UnidadMedidaId = nuevoMiseAndPlace.UnidadMedidaId,
                    StockMinimo = 0
                };

                await _ctx.Insumos.AddAsync(insumoDb);
                await _ctx.SaveChangesAsync();

                var ingredienteDb = new PanComido.Infraestructura.Persistencia.Entidades.Ingrediente
                {
                    IdInsumo = insumoDb.IdArticulo
                };

                await _ctx.Ingredientes.AddAsync(ingredienteDb);
                await _ctx.SaveChangesAsync();

                var ingredientePreparadoDb = new PanComido.Infraestructura.Persistencia.Entidades.IngredientePreparado
                {
                    IdIngrediente = ingredienteDb.IdInsumo,
                    RendimientoBase = nuevoMiseAndPlace.RendimientoBase
                };

                await _ctx.IngredientePreparados.AddAsync(ingredientePreparadoDb);
                await _ctx.SaveChangesAsync();

                foreach (var ing in nuevoMiseAndPlace.Ingredientes)
                {
                    var relacion = new PanComido.Infraestructura.Persistencia.Entidades.IngredienteIngredientePreparado
                    {
                        IngredienteId = ing.IngredienteId,
                        IngredientePreparadoId = ingredientePreparadoDb.IdIngrediente,
                        Cantidad = ing.Cantidad
                    };
                    await _ctx.IngredienteIngredientePreparados.AddAsync(relacion);
                }
                await _ctx.SaveChangesAsync();

                var lote = new PanComido.Infraestructura.Persistencia.Entidades.Lote
                {
                    InsumoId = insumoDb.IdArticulo,
                    BodegaId = nuevoMiseAndPlace.BodegaId,
                    Nombre = nombreLote,
                    Cantidad = nuevoMiseAndPlace.Cantidad,
                    FechaAdquisicion = DateOnly.FromDateTime(DateTime.UtcNow),
                    FechaVencimiento = nuevoMiseAndPlace.FechaVencimiento
                };

                await _ctx.Lotes.AddAsync(lote);
                await _ctx.SaveChangesAsync();

                await transaction.CommitAsync();

                return ingredientePreparadoDb.IdIngrediente;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<MiseAndPlaceListadoDominio>> ObtenerTodosAsync(int restauranteId)
        {
            var ingredientesPreparados = await _ctx.IngredientePreparados
                .Include(ip => ip.IdIngredienteNavigation)
                    .ThenInclude(i => i.IdInsumoNavigation)
                        .ThenInclude(ins => ins.IdArticuloNavigation)
                .Include(ip => ip.IdIngredienteNavigation)
                    .ThenInclude(i => i.IdInsumoNavigation)
                        .ThenInclude(ins => ins.Lotes)
                            .ThenInclude(l => l.Bodega)
                .Include(ip => ip.IdIngredienteNavigation)
                    .ThenInclude(i => i.IdInsumoNavigation)
                        .ThenInclude(ins => ins.UnidadMedida)
                .Include(ip => ip.IdIngredienteNavigation)
                    .ThenInclude(i => i.IdInsumoNavigation)
                        .ThenInclude(ins => ins.CategoriaInsumo)
                .Include(ip => ip.IngredienteIngredientePreparados)
                    .ThenInclude(receta => receta.Ingrediente)
                        .ThenInclude(ing => ing.IdInsumoNavigation)
                            .ThenInclude(ins => ins.IdArticuloNavigation)
                .Include(ip => ip.IngredienteIngredientePreparados)
                    .ThenInclude(receta => receta.Ingrediente)
                        .ThenInclude(ing => ing.IdInsumoNavigation)
                            .ThenInclude(ins => ins.UnidadMedida)
                .Where(ip => ip.IdIngredienteNavigation.IdInsumoNavigation.IdArticuloNavigation.RestauranteId == restauranteId &&
                             !ip.IdIngredienteNavigation.IdInsumoNavigation.IdArticuloNavigation.Eliminado)
                .ToListAsync();

            var resultado = new List<MiseAndPlaceListadoDominio>();

            foreach (var ip in ingredientesPreparados)
            {
                var insumo = ip.IdIngredienteNavigation.IdInsumoNavigation;
                var articulo = insumo.IdArticuloNavigation;

                var recetaDominio = ip.IngredienteIngredientePreparados.Select(r => new RecetaItemDominio
                {
                    IngredienteId = r.IngredienteId,
                    NombreIngrediente = r.Ingrediente.IdInsumoNavigation.IdArticuloNavigation.Nombre,
                    Cantidad = r.Cantidad,
                    UnidadMedida = r.Ingrediente.IdInsumoNavigation.UnidadMedida.Nombre
                }).ToList();

                foreach (var lote in insumo.Lotes)
                {
                    resultado.Add(new MiseAndPlaceListadoDominio
                    {
                        LoteId = lote.Id,
                        ArticuloId = articulo.Id,
                        MiseAndPlaceId = ip.IdIngrediente,
                        Nombre = articulo.Nombre,
                        Descripcion = articulo.Descripcion,
                        Cantidad = lote.Cantidad,
                        FechaVencimiento = lote.FechaVencimiento,
                        UnidadMedida = insumo.UnidadMedida.Nombre,
                        Categoria = insumo.CategoriaInsumo.Descripcion,
                        Bodega = lote.Bodega.Nombre,
                        Receta = recetaDominio
                    });
                }
            }

            return resultado.OrderBy(x => x.FechaVencimiento).ToList();
        }

        public async Task<MiseAndPlaceListadoDominio> ObtenerPorIdAsync(int restauranteId, int miseAndPlaceId)
        {
            var ip = await _ctx.IngredientePreparados
                .Include(i => i.IdIngredienteNavigation)
                    .ThenInclude(i => i.IdInsumoNavigation)
                        .ThenInclude(ins => ins.IdArticuloNavigation)
                .Include(i => i.IdIngredienteNavigation)
                    .ThenInclude(i => i.IdInsumoNavigation)
                        .ThenInclude(ins => ins.Lotes)
                            .ThenInclude(l => l.Bodega)
                .Include(i => i.IdIngredienteNavigation)
                    .ThenInclude(i => i.IdInsumoNavigation)
                        .ThenInclude(ins => ins.UnidadMedida)
                .Include(i => i.IdIngredienteNavigation)
                    .ThenInclude(i => i.IdInsumoNavigation)
                        .ThenInclude(ins => ins.CategoriaInsumo)
                .Include(i => i.IngredienteIngredientePreparados)
                    .ThenInclude(receta => receta.Ingrediente)
                        .ThenInclude(ing => ing.IdInsumoNavigation)
                            .ThenInclude(ins => ins.IdArticuloNavigation)
                .Include(i => i.IngredienteIngredientePreparados)
                    .ThenInclude(receta => receta.Ingrediente)
                        .ThenInclude(ing => ing.IdInsumoNavigation)
                            .ThenInclude(ins => ins.UnidadMedida)
                .FirstOrDefaultAsync(i => 
                    i.IdIngredienteNavigation.IdInsumoNavigation.IdArticuloNavigation.RestauranteId == restauranteId &&
                    i.IdIngrediente == miseAndPlaceId);

            if (ip == null) return null;

            var insumo = ip.IdIngredienteNavigation.IdInsumoNavigation;
            var articulo = insumo.IdArticuloNavigation;

            var recetaDominio = ip.IngredienteIngredientePreparados.Select(r => new RecetaItemDominio
            {
                IngredienteId = r.IngredienteId,
                NombreIngrediente = r.Ingrediente.IdInsumoNavigation.IdArticuloNavigation.Nombre,
                Cantidad = r.Cantidad,
                UnidadMedida = r.Ingrediente.IdInsumoNavigation.UnidadMedida.Nombre
            }).ToList();

            var lastLote = insumo.Lotes.OrderByDescending(l => l.FechaAdquisicion).FirstOrDefault();

            return new MiseAndPlaceListadoDominio
            {
                LoteId = lastLote?.Id ?? 0,
                ArticuloId = articulo.Id,
                MiseAndPlaceId = ip.IdIngrediente,
                Nombre = articulo.Nombre,
                Descripcion = articulo.Descripcion,
                Cantidad = lastLote?.Cantidad ?? 0,
                RendimientoBase = ip.RendimientoBase,
                FechaVencimiento = lastLote?.FechaVencimiento,
                UnidadMedida = insumo.UnidadMedida.Nombre,
                Categoria = insumo.CategoriaInsumo.Descripcion,
                Bodega = lastLote?.Bodega?.Nombre ?? "",
                Receta = recetaDominio
            };
        }

        public async Task<int> ProducirMiseAndPlaceAsync(int restauranteId, int miseAndPlaceId, decimal cantidad, DateOnly fechaVencimiento, int bodegaId, string nombreLote)
        {
            var ingredientePreparadoDb = await _ctx.IngredientePreparados
                .Include(ip => ip.IdIngredienteNavigation)
                .FirstOrDefaultAsync(ip => ip.IdIngrediente == miseAndPlaceId);

            if (ingredientePreparadoDb == null)
            {
                throw new ArgumentException("El ingrediente preparado no existe.");
            }

            var lote = new PanComido.Infraestructura.Persistencia.Entidades.Lote
            {
                InsumoId = ingredientePreparadoDb.IdIngredienteNavigation.IdArticulo,
                BodegaId = bodegaId,
                Nombre = nombreLote,
                Cantidad = cantidad,
                FechaAdquisicion = DateOnly.FromDateTime(DateTime.UtcNow),
                FechaVencimiento = fechaVencimiento
            };

            await _ctx.Lotes.AddAsync(lote);
            await _ctx.SaveChangesAsync();

            return lote.Id;
        }

        public async Task<bool> EliminarMiseAndPlaceAsync(int restauranteId, int miseAndPlaceId)
        {
            var articulo = await _ctx.Articulos
                .FirstOrDefaultAsync(a => a.Id == miseAndPlaceId && a.RestauranteId == restauranteId);

            if (articulo == null || articulo.Eliminado)
                return false;

            articulo.Eliminado = true;
            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ModificarMiseAndPlaceAsync(int restauranteId, int miseAndPlaceId, ModificarMiseAndPlaceDominio datos)
        {
            using var transaction = await _ctx.Database.BeginTransactionAsync();
            try
            {
                var articulo = await _ctx.Articulos.FirstOrDefaultAsync(a => a.Id == miseAndPlaceId && a.RestauranteId == restauranteId);
                var insumo = await _ctx.Insumos.FirstOrDefaultAsync(i => i.IdArticulo == miseAndPlaceId);
                var ingredientePreparado = await _ctx.IngredientePreparados.FirstOrDefaultAsync(ip => ip.IdIngrediente == miseAndPlaceId);
                var lote = await _ctx.Lotes.FirstOrDefaultAsync(l => l.Id == datos.LoteId && l.InsumoId == miseAndPlaceId);

                if (articulo == null || articulo.Eliminado || insumo == null || ingredientePreparado == null || lote == null)
                    return false;

                // 1. Update Articulo, Insumo and IngredientePreparado
                articulo.Nombre = datos.Nombre;
                articulo.Descripcion = datos.Descripcion;
                insumo.CategoriaInsumoId = datos.CategoriaId;
                insumo.UnidadMedidaId = datos.UnidadMedidaId;
                ingredientePreparado.RendimientoBase = datos.RendimientoBase;

                // 2. Update Lote (Solo Bodega y Vencimiento, NO Cantidad para evitar desajustes de stock)
                lote.FechaVencimiento = datos.FechaVencimiento;
                lote.BodegaId = datos.BodegaId;

                await _ctx.SaveChangesAsync();

                // 3. Update Recipe (Delete old, Insert new)
                var recetasViejas = await _ctx.IngredienteIngredientePreparados
                    .Where(r => r.IngredientePreparadoId == miseAndPlaceId)
                    .ToListAsync();
                _ctx.IngredienteIngredientePreparados.RemoveRange(recetasViejas);

                foreach (var ing in datos.Ingredientes)
                {
                    var nuevaReceta = new PanComido.Infraestructura.Persistencia.Entidades.IngredienteIngredientePreparado
                    {
                        IngredienteId = ing.IngredienteId,
                        IngredientePreparadoId = miseAndPlaceId,
                        Cantidad = ing.Cantidad
                    };
                    await _ctx.IngredienteIngredientePreparados.AddAsync(nuevaReceta);
                }

                await _ctx.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
