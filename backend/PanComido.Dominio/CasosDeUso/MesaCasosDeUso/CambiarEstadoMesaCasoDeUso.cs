using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class CambiarEstadoMesaCasoDeUso
    {
        private readonly IMesaRepositorio _mesaRepositorio;
        private readonly ILlamadoNotificador _llamadoNotificador;
        private readonly ILlamadoRepositorio _llamadoRepositorio;

        public CambiarEstadoMesaCasoDeUso(IMesaRepositorio mesaRepositorio, ILlamadoNotificador llamadoNotificador, ILlamadoRepositorio llamadoRepositorio)
        {
            _mesaRepositorio = mesaRepositorio;
            _llamadoNotificador = llamadoNotificador;
            _llamadoRepositorio = llamadoRepositorio;
        }

        public async Task<MesaConPosiciones> EjecutarAsync(int restauranteId, int mesaId, EstadoMesa nuevoEstado)
        {
            MesaConPosiciones mesa = await _mesaRepositorio.ObtenerPorIdAsync(mesaId, restauranteId);

            if (mesa == null)
                throw new ArgumentException("La mesa no existe o no pertenece al restaurante.");

            mesa.EstadoMesa = nuevoEstado;
            await _mesaRepositorio.ActualizarEstadoAsync(mesaId, nuevoEstado);
            if(mesa.EstadoMesa == EstadoMesa.Disponible)
            {
                List<Llamado> llamadosResueltos = await _llamadoRepositorio.ResolverTodosLosPendientesPorMesaAsync(mesaId);
                if (llamadosResueltos.Any())
                    await _llamadoNotificador.NotificarLlamadosResueltosAsync(mesaId, llamadosResueltos);
            }
            return mesa;
        }
    }
}
