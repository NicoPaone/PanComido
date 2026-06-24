using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Entidades;
using PanComido.Infraestructura.Persistencia.Mappers;

using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class MesaRepositorio : IMesaRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly MesaEntityMapper _mapper;
        public MesaRepositorio(AppDbContext ctx, MesaEntityMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }
        private IQueryable<EF.Mesa> BaseQuery(int restauranteId)
        {
            return _ctx.Mesas
               .Where(m => m.Grilla.RestauranteId == restauranteId && m.Activo);
        }
        public async Task<DOM.MesaConPosiciones?> ObtenerPorIdAsync(int id, int restauranteId)
        {
            EF.Mesa mesaEF = await BaseQuery(restauranteId)
               .AsNoTracking()
               .Include(m => m.DimensionMesa)
               .Include(m => m.Mozos)
               .FirstOrDefaultAsync(m => m.Id == id);
            return _mapper.paraDominioCompleto(mesaEF);
        }
        public async Task ActualizarAsync(DOM.Mesa mesaDominio)
        {
            EF.Mesa mesaEF = _mapper.paraEntidad(mesaDominio);
            _ctx.Mesas.Update(mesaEF);
            await _ctx.SaveChangesAsync();
        }
        public async Task<List<MesaConPosiciones>> ObtenerTodasAsync(int restauranteId)
        {
            List<EF.Mesa> mesasEF = await BaseQuery(restauranteId)
               .AsNoTracking()
               .Include(m => m.DimensionMesa)
               .Include(m => m.Mozos)
               .ToListAsync();

            return mesasEF
               .Select(m => _mapper.paraDominioCompleto(m)!)
               .Where(m => m != null)
               .ToList();
        }
        public async Task<List<MesaConPosiciones>> ObtenerOcupadasAsync(int restauranteId)
        {
            List<EF.Mesa> mesasEF = await BaseQuery(restauranteId)
                .AsNoTracking()
                .Include(m => m.DimensionMesa)
                .Include(m => m.Mozos)
                .Where(m => m.EstadoMesaId == (int)DOM.Enums.EstadoMesa.Ocupada)
                .ToListAsync();

            return mesasEF
                .Select(m => _mapper.paraDominioCompleto(m)!)
                .Where(m => m != null)
                .ToList();
        }
        public async Task<List<MesaConPosiciones>> ObtenerDisponiblesAsync(int restauranteId)
        {
            List<EF.Mesa> mesasEF = await BaseQuery(restauranteId)
                .AsNoTracking()
                .Include(m => m.DimensionMesa)
                .Include(m => m.Mozos)
                .Where(m => m.EstadoMesaId == (int)DOM.Enums.EstadoMesa.Disponible)
                .ToListAsync();

            return mesasEF
                .Select(m => _mapper.paraDominioCompleto(m)!)
                .Where(m => m != null)
                .ToList();
        }
        public async Task ActualizarEstadoAsync(int mesaId, DOM.Enums.EstadoMesa nuevoEstado)
        {
            var mesaEF = await _ctx.Mesas.FirstOrDefaultAsync(m => m.Id == mesaId);
            if (mesaEF == null) return;

            mesaEF.EstadoMesaId = (int)nuevoEstado;
            await _ctx.SaveChangesAsync();
        }

        public async Task<List<int>> ObtenerMozoIdsPorMesaAsync(int mesaId)
        {
            return await _ctx.Mozos
                .Where(m => m.Mesas.Any(mesa => mesa.Id == mesaId))
                .Select(m => m.IdEmpleado)
                .ToListAsync();
        }

        public async Task AsignarMozosAsync(int restauranteId, int mesaId, List<int> mozosIds)
        {
            var mesa = await _ctx.Mesas
                .Include(m => m.Mozos)
                .FirstOrDefaultAsync(m => m.Id == mesaId && m.Grilla.RestauranteId == restauranteId);
                
            if (mesa == null) throw new Exception($"La mesa {mesaId} no existe en este restaurante.");

            var mozosExistentes = mesa.Mozos.Select(m => m.IdEmpleado).ToList();
            var nuevosMozosIds = mozosIds.Except(mozosExistentes).ToList();
            var mozosAEliminarIds = mozosExistentes.Except(mozosIds).ToList();

            bool cambiosRealizados = false;

            if (mozosAEliminarIds.Any())
            {
                var mozosAEliminar = mesa.Mozos.Where(m => mozosAEliminarIds.Contains(m.IdEmpleado)).ToList();
                foreach (var mozo in mozosAEliminar)
                {
                    mesa.Mozos.Remove(mozo);
                }
                cambiosRealizados = true;
            }

            if (nuevosMozosIds.Any())
            {
                var mozos = await _ctx.Mozos.Where(m => nuevosMozosIds.Contains(m.IdEmpleado)).ToListAsync();
                if (mozos.Count != nuevosMozosIds.Count)
                {
                    throw new Exception("Uno o más IDs de mozo enviados no existen en la base de datos.");
                }
                foreach (var mozo in mozos)
                {
                    mesa.Mozos.Add(mozo);
                }
                cambiosRealizados = true;
            }

            if (cambiosRealizados)
            {
                await _ctx.SaveChangesAsync();
            }
        }

        public async Task DesasignarMozoAsync(int restauranteId, int mesaId, int mozoId)
        {
            var mesa = await _ctx.Mesas
                .Include(m => m.Mozos)
                .FirstOrDefaultAsync(m => m.Id == mesaId && m.Grilla.RestauranteId == restauranteId);
                
            if (mesa == null) return;

            var mozo = mesa.Mozos.FirstOrDefault(m => m.IdEmpleado == mozoId);
            if (mozo != null)
            {
                mesa.Mozos.Remove(mozo);
                await _ctx.SaveChangesAsync();
            }
        }

        public async Task GuardarMapaMasivoAsync(int restauranteId, List<DOM.MesaMapaDominio> mesasDominio)
        {
            var grilla = await _ctx.Grillas.FirstAsync(g => g.RestauranteId == restauranteId);

            var mesasNuevas = mesasDominio.Where(m => m.Id <= 0).ToList();
            var mesasActualizar = mesasDominio.Where(m => m.Id > 0).ToList();

            foreach (var mesaNueva in mesasNuevas)
            {
                int dimensionId = mesaNueva.DimensionMesaId;
                if (dimensionId <= 0 && !string.IsNullOrEmpty(mesaNueva.Forma))
                {
                    var dim = await _ctx.DimensionMesas.FirstOrDefaultAsync(d => d.Forma == mesaNueva.Forma);
                    if (dim != null) dimensionId = dim.Id;
                }

                var nuevaMesaEF = new EF.Mesa
                {
                    GrillaId = grilla.Id,
                    Numero = mesaNueva.Numero,
                    CantPersonasMax = mesaNueva.CantPersonasMax,
                    EstadoMesaId = (int)mesaNueva.EstadoMesa,
                    PosicionXInicio = mesaNueva.PosicionXInicio,
                    PosicionXFin = mesaNueva.PosicionXFin,
                    PosicionYInicio = mesaNueva.PosicionYInicio,
                    PosicionYFin = mesaNueva.PosicionYFin,
                    DimensionMesaId = dimensionId,
                    TipoElemento = mesaNueva.TipoElemento,
                    Color = mesaNueva.Color,
                    TextoObjeto = mesaNueva.TextoObjeto
                };
                _ctx.Mesas.Add(nuevaMesaEF);
            }

            if (mesasActualizar.Any())
            {
                var idsActualizar = mesasActualizar.Select(m => m.Id).ToList();

                var mesasExistentes = await _ctx.Mesas
                    .Where(m => idsActualizar.Contains(m.Id) && m.Grilla.RestauranteId == restauranteId)
                    .ToListAsync();

                foreach (var mesaExistente in mesasExistentes)
                {
                    var mapDatos = mesasActualizar.First(m => m.Id == mesaExistente.Id);

                    mesaExistente.Numero = mapDatos.Numero;
                    mesaExistente.CantPersonasMax = mapDatos.CantPersonasMax;
                    mesaExistente.PosicionXInicio = mapDatos.PosicionXInicio;
                    mesaExistente.PosicionXFin = mapDatos.PosicionXFin;
                    mesaExistente.PosicionYInicio = mapDatos.PosicionYInicio;
                    mesaExistente.PosicionYFin = mapDatos.PosicionYFin;
                    mesaExistente.DimensionMesaId = mapDatos.DimensionMesaId;
                    mesaExistente.EstadoMesaId = (int)mapDatos.EstadoMesa;
                    mesaExistente.TipoElemento = mapDatos.TipoElemento;
                    mesaExistente.Color = mapDatos.Color;
                    mesaExistente.TextoObjeto = mapDatos.TextoObjeto;

                    _ctx.Mesas.Update(mesaExistente);
                }
            }

            var idsRecibidos = mesasActualizar.Select(m => m.Id).ToList();

            var idsMesasParaEliminar = await _ctx.Mesas
                .Where(m => m.Grilla.RestauranteId == restauranteId && !idsRecibidos.Contains(m.Id) && m.Activo)
                .Select(m => m.Id)
                .ToListAsync();

            if (idsMesasParaEliminar.Any())
            {
                var tieneComandasActivas = await _ctx.Mesas
                    .Where(m => idsMesasParaEliminar.Contains(m.Id))
                    .SelectMany(m => m.Comanda)
                    .AnyAsync(c => c.EstadoComandaId != 4);

                if (tieneComandasActivas)
                {
                    throw new InvalidOperationException("No se puede guardar el mapa: se intentó eliminar una mesa que tiene una comanda activa.");
                }

                var tieneMozosAsignados = await _ctx.Mesas
                    .Where(m => idsMesasParaEliminar.Contains(m.Id))
                    .AnyAsync(m => m.Mozos.Any());

                if (tieneMozosAsignados)
                {
                    throw new InvalidOperationException("No se puede guardar el mapa: se intentó eliminar una mesa que tiene mozos asignados. Desasigná a los mozos antes de eliminarla.");
                }

                var mesasParaEliminar = await _ctx.Mesas
                    .Where(m => idsMesasParaEliminar.Contains(m.Id))
                    .ToListAsync();

                foreach(var mesaParaEliminar in mesasParaEliminar)
                {
                    mesaParaEliminar.Activo = false;
                }
                
                _ctx.Mesas.UpdateRange(mesasParaEliminar);
            }

            await _ctx.SaveChangesAsync();
        }

        public async Task<List<DOM.Empleado>> ObtenerTodosLosMozosAsync(int restauranteId)
        {
            var empleados = await _ctx.Mozos
                .Include(m => m.IdEmpleadoNavigation)
                .Where(m => m.IdEmpleadoNavigation.RestauranteId == restauranteId && m.Activo == true)
                .Select(m => m.IdEmpleadoNavigation)
                .ToListAsync();

            return empleados.Select(e => new DOM.Empleado
            {
                Id = e.Id,
                RestauranteId = e.RestauranteId,
                Nombre = e.Nombre,
                Email = e.Email,
                ContraseniaHash = e.Contrasena,
                Estado = e.Estado
            }).ToList();
        }
    }
}