using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class AsignarMozosMesaCasoDeUso
    {
        private readonly IMesaRepositorio _mesaRepositorio;
        private readonly IMesaNotificador _mesaNotificador;

        public AsignarMozosMesaCasoDeUso(IMesaRepositorio mesaRepositorio, IMesaNotificador mesaNotificador)
        {
            _mesaRepositorio = mesaRepositorio;
            _mesaNotificador = mesaNotificador;
        }

        public async Task<MesaConPosiciones> EjecutarAsync(int restauranteId, int mesaId, List<int> mozosIds)
        {
            MesaConPosiciones mesa = await _mesaRepositorio.ObtenerPorIdAsync(mesaId, restauranteId);
            if (mesa == null) throw new ArgumentException("La mesa no existe o no pertenece al restaurante.");

            var mozosAAgregar = mozosIds.Except(mesa.MozosAsignadosIds).ToList();
            var mozosAEliminar = mesa.MozosAsignadosIds.Except(mozosIds).ToList();

            await _mesaRepositorio.AsignarMozosAsync(mesaId, mozosAAgregar, mozosAEliminar);

            mesa.MozosAsignadosIds = mozosIds;
            await _mesaNotificador.NotificarMesaActualizadaAsync(mesa, restauranteId);
            return mesa;
        }
    }
}
    