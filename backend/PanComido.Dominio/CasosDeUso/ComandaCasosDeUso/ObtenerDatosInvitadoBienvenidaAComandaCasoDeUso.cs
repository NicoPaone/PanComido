using Microsoft.Extensions.Logging;
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

        private readonly ILogger<ObtenerDatosInvitadoBienvenidaAComandaCasoDeUso> _logger;

        public ObtenerDatosInvitadoBienvenidaAComandaCasoDeUso(
            IComandaRepositorio comandaRepositorio,
            IRestauranteRepositorio restauranteRepositorio,
            IMesaRepositorio mesaRepositorio,
            ILogger<ObtenerDatosInvitadoBienvenidaAComandaCasoDeUso> logger)
        {
            _comandaRepositorio = comandaRepositorio;
            _restauranteRepositorio = restauranteRepositorio;
            _mesaRepositorio = mesaRepositorio;
            _logger = logger;
        }
        public async Task<BienvenidaDatosInvitadoComanda> EjecutarAsync(int comandaId)
        {

            Comanda comanda = await ObtenerYValidarComandaAInvitar(comandaId);

            MesaConPosiciones mesa = await _mesaRepositorio.ObtenerPorIdAsync(comanda.MesaId, comanda.RestauranteId);

            return new BienvenidaDatosInvitadoComanda
            {
                IdComanda = comanda.Id,
                CantComensales = comanda.CantComensales,
                Mesa = mesa,
                RestauranteId = comanda.RestauranteId
            };
        }

        private async Task<Comanda> ObtenerYValidarComandaAInvitar(int comandaId)
        {
            Comanda comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);

            if (comanda == null)
            {
                _logger.LogWarning("Intento de acceso a invitación con una comanda inexistente. ComandaId: {ComandaId}", comandaId);
                throw new KeyNotFoundException("La comanda de invitación no existe.");
            }

            if (comanda.Estado == EstadoComanda.Finalizada)
            {
                _logger.LogWarning("Intento de ingreso como invitado a una mesa con comanda ya finalizada. ComandaId: {ComandaId}", comandaId);
                throw new InvalidOperationException("Esta mesa ya ha finalizado su pedido.");
            }

            return comanda;
        }
    }
}
