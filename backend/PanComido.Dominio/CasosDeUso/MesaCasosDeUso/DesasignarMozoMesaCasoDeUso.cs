using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class DesasignarMozoMesaCasoDeUso
    {
        private readonly IMesaRepositorio _mesaRepositorio;
        private readonly IMesaNotificador _mesaNotificador;

        public DesasignarMozoMesaCasoDeUso(IMesaRepositorio mesaRepositorio, IMesaNotificador mesaNotificador)
        {
            _mesaRepositorio = mesaRepositorio;
            _mesaNotificador = mesaNotificador;
        }

        public async Task<MesaConPosiciones> EjecutarAsync(int restauranteId, int mesaId, int mozoId)
        {
            MesaConPosiciones mesa = await _mesaRepositorio.ObtenerPorIdAsync(mesaId, restauranteId);
            if (mesa == null) throw new ArgumentException("La mesa no existe o no pertenece al restaurante.");

            await _mesaRepositorio.DesasignarMozoAsync(mesaId, mozoId);

            mesa.MozosAsignadosIds.Remove(mozoId);
            await _mesaNotificador.NotificarMesaActualizadaAsync(mesa, restauranteId);
            return mesa;
        }
    }
}
