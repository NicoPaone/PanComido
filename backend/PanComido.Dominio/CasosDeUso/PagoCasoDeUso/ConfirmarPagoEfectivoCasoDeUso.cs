using PanComido.Dominio.CasosDeUso.ComandaCasosDeUso;
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
    public class ConfirmarPagoEfectivoCasoDeUso
    {
        private readonly IPagoRepositorio _pagoRepositorio;
        private readonly IComandaRepositorio _comandaRepositorio;
        private readonly ILlamadoRepositorio _llamadoRepositorio;
        private readonly ICalcularTotalComandaServicio _calcularTotalComandaServicio;

        public ConfirmarPagoEfectivoCasoDeUso(IPagoRepositorio pagoRepositorio, IComandaRepositorio comandaRepositorio, ILlamadoRepositorio llamadoRepositorio, ICalcularTotalComandaServicio calcularTotalComandaServicio)
        {
            _pagoRepositorio = pagoRepositorio;
            _comandaRepositorio = comandaRepositorio;
            _llamadoRepositorio = llamadoRepositorio;
            _calcularTotalComandaServicio = calcularTotalComandaServicio;
        }

        public async Task<Pago> EjecutarAsync(int comandaId, int restauranteId)
        {
            var comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);
            if (comanda == null) throw new KeyNotFoundException("Comanda no encontrada");
            if (comanda.Estado != EstadoComanda.EnEspera)
                throw new ArgumentException("La comanda no está esperando pago.");

            decimal totalComanda = _calcularTotalComandaServicio.CalcularTotal(comanda);

            Pago pagoExistente = await _pagoRepositorio.ObtenerPagoPorComandaIdAsync(comandaId);
            if (pagoExistente != null && pagoExistente.EstadoPago == EstadoPago.Confirmado) throw new InvalidOperationException("El pago ya fue confirmado");

            Pago pago = new Pago
            {
                MetodoDePago = MetodoPago.Efectivo,
                Total = totalComanda,
                ComandaId = comandaId,
                EstadoPago = EstadoPago.Confirmado
            };

            Pago pagoCreado = await _pagoRepositorio.CrearPagoAsync(pago);

            comanda.Estado = EstadoComanda.Finalizada;
            comanda.HoraFin = DateTime.Now;
            await _comandaRepositorio.ActualizarAsync(comanda);

            await _llamadoRepositorio.ResolverLlamadoPorMesaYCategoriaAsync(comanda.MesaId, 7);
            return pagoCreado;
        }
    }
}
