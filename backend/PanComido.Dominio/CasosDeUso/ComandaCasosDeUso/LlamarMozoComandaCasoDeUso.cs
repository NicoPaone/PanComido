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
            var comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);

            if (comanda == null || comanda.RestauranteId != restauranteId)
            {
                return;
            }

            int mozoId = await _mozoRepositorio.ObtenerMozoAsignadoAMesaAsync(comanda.MesaId);

            if (mozoId > 0)
            {
                List<int> mozoIds = new List<int> { mozoId };

                await _notificador.NotificarLlamadoCocinaAsync(comanda, mozoIds);
            }
        }
    }
}