using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Dominio.Interfaces.Servicios.MercadoPago;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.PagoCasoDeUso
{
    public class CrearPreferenciaMPCasoDeUso
    {
        private readonly IComandaRepositorio _comandaRepositorio;
        private readonly IMercadoPagoServicio _mercadoPagoServicio;
        private readonly ICalcularTotalComandaServicio _calcularTotalComandaServicio;
        private readonly IRestauranteRepositorio _restauranteRepositorio;
        private readonly IPagoRepositorio _pagoRepositorio;


        public CrearPreferenciaMPCasoDeUso(IComandaRepositorio comandaRepositorio, IMercadoPagoServicio mercadoPagoServicio, ICalcularTotalComandaServicio calcularTotalComandaServicio,IRestauranteRepositorio restauranteRepositorio, IPagoRepositorio pagoRepositorio)
        {
            _comandaRepositorio = comandaRepositorio;
            _mercadoPagoServicio = mercadoPagoServicio;
            _calcularTotalComandaServicio = calcularTotalComandaServicio;
            _restauranteRepositorio = restauranteRepositorio;
            _pagoRepositorio = pagoRepositorio;
        }

        public async Task<string> EjecutarAsync(int comandaId, int restauranteId)
        {
         var comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);
         if (comanda == null || comanda.RestauranteId != restauranteId) throw new KeyNotFoundException("Comanda no encontrada");
         if (comanda.Estado != EstadoComanda.EnEspera)
            throw new ArgumentException("La comanda no está esperando pago.");

         decimal totalComanda = _calcularTotalComandaServicio.CalcularTotal(comanda);

            string externalReference = $"Comanda-{comandaId}";
            Restaurante restaurante = await _restauranteRepositorio.ObtenerDatosDelLocalAsync(restauranteId);

            Pago pagoExistente = await _pagoRepositorio.ObtenerPagoPorComandaIdAsync(comandaId);
            if (pagoExistente != null && pagoExistente.EstadoPago == EstadoPago.Confirmado) throw new InvalidOperationException("El pago ya fue confirmado");
            
            string descripcion = $"Pago a {restaurante.Nombre}";

            string initPoint = await _mercadoPagoServicio.CrearPreferenciaAsync(externalReference, totalComanda, descripcion);

            Pago pago = new Pago
            {
                MetodoDePago = MetodoPago.MercadoPago,
                Total = totalComanda,
                ComandaId = comandaId,
                ExternalReference = externalReference,
                EstadoPago = EstadoPago.Pendiente
            };

            await _pagoRepositorio.CrearPagoAsync(pago);
            return initPoint;
        }
    }
}
