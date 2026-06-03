using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.CasosDeUso.ComandaCasosDeUso
{
   public class ModificarEstadoComandaCasoDeUso
   {
      private readonly IComandaRepositorio _comandaRepositorio;
        private readonly IComandaNotificador _comandaNotificador;
        private readonly IMesaRepositorio _mesaRepositorio;

        public ModificarEstadoComandaCasoDeUso(
            IComandaRepositorio comandaRepositorio,
            IComandaNotificador comandaNotificador,
            IMesaRepositorio mesaRepositorio)
        {
            _comandaRepositorio = comandaRepositorio;
            _comandaNotificador = comandaNotificador;
            _mesaRepositorio = mesaRepositorio;
        }

        public async Task<Comanda?> EjecutarAsync(int comandaId, int estadoId)
        {
            var resultado = await _comandaRepositorio.ModificarEstadoComandaAsync(comandaId, estadoId);
            if (resultado == null) throw new KeyNotFoundException("No se encontró una comanda activa para esa mesa.");
            Comanda comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);
            var mozoId = await _mesaRepositorio.ObtenerMozoIdsPorMesaAsync(comanda.MesaId);
            await _comandaNotificador.NotificarEstadoModificadoAsync(comanda, mozoId);

            return comanda;
        }
    }
}
