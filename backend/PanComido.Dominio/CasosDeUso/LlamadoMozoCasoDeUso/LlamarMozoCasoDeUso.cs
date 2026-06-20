using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Dominio.CasosDeUso.LlamadoMozoCasoDeUso
{
    public class LlamarMozoCasoDeUso
    {
        private readonly IMozoRepositorio _mozoRepositorio;
        private readonly ILlamadoRepositorio _llamadoRepositorio;
        private readonly ILlamadoNotificador _llamadoNotificador;
        private readonly IMesaRepositorio _mesaRepositorio;
        private readonly ILogger<LlamarMozoCasoDeUso> _logger;

        public LlamarMozoCasoDeUso(
            IMozoRepositorio mozoRepositorio,
            ILlamadoRepositorio llamadoRepositorio,
            ILlamadoNotificador llamadoNotificador,
            IMesaRepositorio mesaRepositorio,
            ILogger<LlamarMozoCasoDeUso> logger)
        {
            _mozoRepositorio = mozoRepositorio;
            _llamadoRepositorio = llamadoRepositorio;
            _llamadoNotificador = llamadoNotificador;
            _mesaRepositorio = mesaRepositorio;
            _logger = logger;
        }

        public async Task<DOM.Llamado> EjecutarAsync(int restauranteId, int mesaId, int categoriaLlamadoId, string? descripcion)
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


            var llamado = new DOM.Llamado
            {
                MozoId = mozoId,
                MesaId = mesaId,
                CategoriaLlamadoId = categoriaLlamadoId,
                Descripcion = descripcion,
                Resuelto = false
            };

            var llamadoGuardado = await _llamadoRepositorio.crearLlamadoAsync(llamado);

            await _llamadoNotificador.NotificarLlamadoAsync(llamadoGuardado);

            _logger.LogInformation("Llamado creado. LlamadoId: {LlamadoId}, MesaId: {MesaId}, MozoId: {MozoId}, CategoriaId: {CategoriaId}", llamadoGuardado.Id, mesaId, mozoId, categoriaLlamadoId);
            return llamadoGuardado;
        }

    }
}
