using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Dominio.CasosDeUso.LlamadoMozoCasoDeUso
{
    public class LlamarMozoCasoDeUso
    {
        private readonly IMozoRepositorio _mozoRepositorio;
        private readonly IMesaRepositorio _mesaRepositorio;
        private readonly ICrearLlamadoServicio _crearLlamadoServicio;
        private readonly ILogger<LlamarMozoCasoDeUso> _logger;

        public LlamarMozoCasoDeUso(
            IMozoRepositorio mozoRepositorio,
            IMesaRepositorio mesaRepositorio,
            ICrearLlamadoServicio crearLlamadoServicio,
            ILogger<LlamarMozoCasoDeUso> logger)
        {
            _mozoRepositorio = mozoRepositorio;
            _mesaRepositorio = mesaRepositorio;
            _crearLlamadoServicio = crearLlamadoServicio;
            _logger = logger;
        }

        public async Task<DOM.Llamado> EjecutarAsync(int restauranteId, int mesaId, CategoriaLlamado categoriaLlamadoId, string? descripcion)
        {
            int mozoId = await _mozoRepositorio.ObtenerMozoAsignadoAMesaAsync(mesaId);
            var mesaObtenidaId = await _mesaRepositorio.ObtenerPorIdAsync(mesaId, restauranteId);
            if (mozoId == 0)
            {
                _logger.LogWarning("No hay mozo asignado a la mesa. MesaId: {MesaId}, RestauranteId: {RestauranteId}", mesaId, restauranteId);
                throw new KeyNotFoundException("No se encontro un mozo asignado a esta mesa.");
            }

            if (mesaObtenidaId == null)
            {
                _logger.LogWarning("Mesa no encontrada. MesaId: {MesaId}, RestauranteId: {RestauranteId}", mesaId, restauranteId);
                throw new KeyNotFoundException("No se encontro la mesa.");
            }

              DOM.Llamado llamadoGuardado = await _crearLlamadoServicio.CrearYNotificarAsync(mozoId, mesaId, mesaObtenidaId.Numero, categoriaLlamadoId, descripcion);

            _logger.LogInformation("Llamado creado. LlamadoId: {LlamadoId}, MesaId: {MesaId}, MozoId: {MozoId}, CategoriaId: {CategoriaId}", llamadoGuardado.Id, mesaId, mozoId, categoriaLlamadoId);
            return llamadoGuardado;
        }

    }
}
