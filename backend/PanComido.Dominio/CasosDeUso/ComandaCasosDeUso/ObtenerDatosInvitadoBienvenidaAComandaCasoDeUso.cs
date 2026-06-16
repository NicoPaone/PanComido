using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ComandaCasosDeUso
{
    public class ObtenerDatosInvitadoBienvenidaAComandaCasoDeUso
    {
        private readonly IComandaRepositorio _comandaRepositorio;
        private readonly IRestauranteRepositorio _restauranteRepositorio;
        private readonly IMesaRepositorio _mesaRepositorio;

        public ObtenerDatosInvitadoBienvenidaAComandaCasoDeUso(
            IComandaRepositorio comandaRepositorio,
            IRestauranteRepositorio restauranteRepositorio,
            IMesaRepositorio mesaRepositorio)
        {
            _comandaRepositorio = comandaRepositorio;
            _restauranteRepositorio = restauranteRepositorio;
            _mesaRepositorio = mesaRepositorio;
        }
        public async Task<BienvenidaDatosInvitadoComanda> EjecutarAsync(int comandaId)
        {

            Comanda comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);

            if (comanda == null)
                throw new KeyNotFoundException("La comanda de invitación no existe.");

            if (comanda.Estado == EstadoComanda.Finalizada)
                throw new InvalidOperationException("Esta mesa ya ha finalizado su pedido.");

            Restaurante restaurante = await _restauranteRepositorio.ObtenerDatosDelLocalAsync(comanda.RestauranteId);
            MesaConPosiciones mesa = await _mesaRepositorio.ObtenerPorIdAsync(comanda.MesaId, comanda.RestauranteId);

            return new BienvenidaDatosInvitadoComanda
            {
                IdComanda = comanda.Id,
                CantComensales = comanda.CantComensales,
                Mesa = mesa,
                RestauranteDatos = restaurante
            };
        }
    }
}
