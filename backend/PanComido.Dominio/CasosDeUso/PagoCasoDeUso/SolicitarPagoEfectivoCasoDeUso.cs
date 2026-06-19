using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.PagoCasoDeUso
{
    public class SolicitarPagoEfectivoCasoDeUso
    {
        private readonly IComandaRepositorio _comandaRepositorio;
        private readonly ILlamadoRepositorio _llamadoRepositorio;
        private readonly ILlamadoNotificador _llamadoNotificador;

        public SolicitarPagoEfectivoCasoDeUso(IComandaRepositorio comandaRepositorio, ILlamadoRepositorio llamadoRepositorio,
            ILlamadoNotificador llamadoNotificador)
        {
            _comandaRepositorio = comandaRepositorio;
            _llamadoRepositorio = llamadoRepositorio;
            _llamadoNotificador = llamadoNotificador;
        }

        public async Task<Llamado> EjecutarAsync(int comandaId, int restauranteId)
        {
            Comanda comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);
            if (comanda == null || comanda.RestauranteId != restauranteId)
            {
                throw new KeyNotFoundException("Comanda no encontrada para el restaurante especificado.");
            }
            if(comanda.Estado != EstadoComanda.EnEspera)
            {
                throw new ArgumentException("La comanda no está esperando pago.");
            }

            comanda.Estado = EstadoComanda.EnEspera;
            await _comandaRepositorio.ActualizarAsync(comanda);

            Llamado llamado = new Llamado
            {
                MozoId = comanda.MozoId,
                MesaId = comanda.MesaId,
                CategoriaLlamadoId = 7,
                Descripcion = "El comensal ha solicitado el pago en efectivo.",
                Resuelto = false
            };

            Llamado llamadoCreado = await _llamadoRepositorio.crearLlamadoAsync(llamado);
            await _llamadoNotificador.NotificarLlamadoAsync(llamadoCreado);
            return llamadoCreado;
        }
    }
}
