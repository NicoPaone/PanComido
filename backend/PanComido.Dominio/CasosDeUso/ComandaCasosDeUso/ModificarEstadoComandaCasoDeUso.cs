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
            if (estadoId == (int)EstadoComanda.EnPreparacion && comanda.Estado != EstadoComanda.Nueva) throw new ArgumentException("La comanda se encuentra en un estado en el que no puede pasar a 'En preparación'");


            Comanda resultado = await _comandaRepositorio.ModificarEstadoComandaAsync(comandaId, estadoId);

            Comanda comandaModificada = await _comandaRepositorio.ObtenerComandaPorIdAsync(resultado.Id);
            if (comandaModificada == null) throw new KeyNotFoundException("No se encontró una comanda activa para esa mesa.");
            var mozoId = await _mesaRepositorio.ObtenerMozoIdsPorMesaAsync(comandaModificada.MesaId);
            await _comandaNotificador.NotificarEstadoModificadoAsync(comandaModificada, mozoId);

            Comanda comandaRetorno = new Comanda
            {
                Id = comandaModificada.Id,
                MesaId = comandaModificada.MesaId,
                NumeroDeMesa = comandaModificada.NumeroDeMesa,
                RestauranteId = comandaModificada.RestauranteId,
                Estado = comandaModificada.Estado,
                CantComensales = comandaModificada.CantComensales,
                HoraInicio = comandaModificada.HoraInicio,
                HoraFin = comandaModificada.HoraFin,
                HoraUltimoCambioEstado = comandaModificada.HoraUltimoCambioEstado,
                Items = comandaModificada.Items.Where(i => i.Articulo is Plato).ToList(),
                MozoId = comandaModificada.MozoId
            };

            return comandaRetorno;
        }
    }
}
