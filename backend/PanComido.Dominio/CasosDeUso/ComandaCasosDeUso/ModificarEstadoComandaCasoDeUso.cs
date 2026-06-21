using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
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
            Comanda comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);

            if (comanda.Estado == EstadoComanda.Finalizada || comanda.Estado == EstadoComanda.Abierta) throw new ArgumentException("La comanda se encuentra en un estado que no puede ser cambiado desde esta acción");

            var resultado = await _comandaRepositorio.ModificarEstadoComandaAsync(comandaId, estadoId);
            if (resultado == null) throw new KeyNotFoundException("No se encontró una comanda activa para esa mesa.");
            var mozoId = await _mesaRepositorio.ObtenerMozoIdsPorMesaAsync(comanda.MesaId);
            await _comandaNotificador.NotificarEstadoModificadoAsync(comanda, mozoId);

            comanda.Items = comanda.Items.Where(i => i.Articulo is Plato).ToList();

            return comanda;
        }
    }
}
