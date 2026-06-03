using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class OcuparMesaCasoDeUso
    {
        private readonly IMesaRepositorio _mesaRepositorio;
        private readonly IComandaRepositorio _comandaRepositorio;

        public OcuparMesaCasoDeUso(IMesaRepositorio mesaRepositorio, IComandaRepositorio comandaRepositorio)
        {
            _mesaRepositorio = mesaRepositorio;
            _comandaRepositorio = comandaRepositorio;
        }

        public async Task<MesaConPosiciones> EjecutarAsync(int restauranteId, int mesaId, int cantComensales)
        {
            MesaConPosiciones mesa = await _mesaRepositorio.ObtenerPorIdAsync(mesaId, restauranteId);

            if (mesa == null)
                throw new ArgumentException("La mesa no existe o no pertenece al restaurante.");
            if (mesa.EstadoMesa != EstadoMesa.Disponible)
                throw new InvalidOperationException("La mesa no está disponible para ser ocupada.");
            if (cantComensales > mesa.CantPersonasMax)
                throw new InvalidOperationException("La cantidad de comensales excede la capacidad máxima de la mesa.");

            mesa.EstadoMesa = EstadoMesa.Ocupada;
            await _mesaRepositorio.ActualizarEstadoAsync(mesaId,EstadoMesa.Ocupada);

            // signalR para informar al gerente

            Comanda nuevaComanda = new Comanda
            {
                MesaId = mesa.Id,
                RestauranteId = restauranteId,
                Estado = EstadoComanda.Abierta,
                CantComensales = cantComensales,
                HoraInicio = DateTime.Now
            };

            int idComanda = await _comandaRepositorio.CrearAsync(nuevaComanda);

            mesa.idComanda = idComanda;
            return mesa;

        }
    }
}
