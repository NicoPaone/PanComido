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

        public ModificarEstadoComandaCasoDeUso(IComandaRepositorio comandaRepositorio, IComandaNotificador comandaNotificador)
      {
         _comandaRepositorio = comandaRepositorio;
         _comandaNotificador = comandaNotificador;
        }

        public async Task<Comanda?> EjecutarAsync(int mesaId, int estadoId)
        {
            Console.Write("Llego????");
            await _comandaRepositorio.ModificarEstadoComandaAsync(mesaId, estadoId);
            Comanda comanda = await _comandaRepositorio.ObtenerComandaPorIdMesaAsync(mesaId);
            await _comandaNotificador.NotificarEstadoModificadoAsync(comanda);

            return comanda;

        }
    }
}
