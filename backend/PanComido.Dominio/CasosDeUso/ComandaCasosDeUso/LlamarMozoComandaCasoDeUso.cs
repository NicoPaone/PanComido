using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ComandaCasosDeUso
{
    public class LlamarMozoComandaCasoDeUso
    {
        private readonly IComandaRepositorio _comandaRepositorio;
        private readonly IMozoRepositorio _mozoRepositorio;
        private readonly IComandaNotificador _notificador;

        public LlamarMozoComandaCasoDeUso(
            IComandaRepositorio comandaRepositorio,
            IMozoRepositorio mozoRepositorio,
            IComandaNotificador notificador)
        {
            _comandaRepositorio = comandaRepositorio;
            _mozoRepositorio = mozoRepositorio;
            _notificador = notificador;
        }

        public async Task EjecutarAsync(int restauranteId, int comandaId)
        {
            // 1. Buscamos la comanda usando el método intacto de tu interfaz
            var comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);

            // Validamos que exista y que pertenezca al restaurante que hace la petición
            if (comanda == null || comanda.RestauranteId != restauranteId)
            {
                return;
            }

            // 2. Buscamos al mozo usando el repositorio sin modificar (devuelve un int)
            int mozoId = await _mozoRepositorio.ObtenerMozoAsignadoAMesaAsync(comanda.MesaId);

            if (mozoId > 0)
            {
                // 3. Como SignalR pide una List<int>, metemos ese único ID adentro de una lista nueva
                List<int> mozoIds = new List<int> { mozoId };

                // 4. Disparamos la notificación
                await _notificador.NotificarLlamadoCocinaAsync(comanda, mozoIds);
            }
        }
    }
}