using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.CierreCajaCasoDeUso
{
    public class GenerarCierreDeCajaCasoDeUso
    {
        private readonly IPagoRepositorio _pagoRepositorio;
        private readonly ITurnoLaboralRepositorio _turnoLaboralRepositorio;
        private readonly ICalculadorVentanaTurnoServicio _calculadorVentanaTurnoServicio;
        private readonly ICierreCajaRepositorio _cierreCajaRepositorio;

        public GenerarCierreDeCajaCasoDeUso(
            IPagoRepositorio pagoRepositorio,
            ITurnoLaboralRepositorio turnoLaboralRepositorio,
            ICalculadorVentanaTurnoServicio calculadorVentanaTurnoServicio,
            ICierreCajaRepositorio cierreCajaRepositorio)
        {
            _pagoRepositorio = pagoRepositorio;
            _turnoLaboralRepositorio = turnoLaboralRepositorio;
            _calculadorVentanaTurnoServicio = calculadorVentanaTurnoServicio;
            _cierreCajaRepositorio = cierreCajaRepositorio;
        }

        public async Task<Cierre> EjecutarAsync(int restauranteId, int turnoLaboralId, decimal conteoCaja)
        {
            var turnos = await _turnoLaboralRepositorio.ObtenerTurnosLaboralesAsync(restauranteId);
            var turno = turnos.FirstOrDefault(t => t.Id == turnoLaboralId);
            if (turno == null) throw new KeyNotFoundException("Turno no encontrado.");

            var ventana = _calculadorVentanaTurnoServicio.CalcularVentana(turno, DateTime.Now);
            var fecha = DateOnly.FromDateTime(ventana.Inicio);

            var cierresExistentes = await _cierreCajaRepositorio.ObtenerCierresDeCajaAsync(restauranteId);
            if (cierresExistentes.Any(c => c.TurnoLaboralId == turnoLaboralId && c.Fecha == fecha))
                throw new InvalidOperationException("Este turno ya fue cerrado.");

            var pagos = await _pagoRepositorio.ObtenerPagosParaCierreAsync(restauranteId, ventana.Inicio, ventana.Fin);

            var totales = CalcularTotalesPorMetodo(pagos);

            decimal diferencia = conteoCaja - totales.Efectivo;
            decimal sobrante = diferencia > 0 ? diferencia : 0;

            var cierre = new Cierre
            {
                RestauranteId = restauranteId,
                TurnoLaboralId = turnoLaboralId,
                Diferencia = diferencia,
                Sobrante = sobrante,
                TotalEfectivo = totales.Efectivo,
                TotalTarjeta = totales.Tarjeta,
                TotalTransferencia = totales.Transferencia,
                TotalMercadoPago = totales.MercadoPago,
                Fecha = fecha
            };

            return await _cierreCajaRepositorio.CrearCierreDeCajaAsync(cierre, pagos.Select(p => p.PagoId).ToList());
        }

        private static (decimal Efectivo, decimal Tarjeta, decimal Transferencia, decimal MercadoPago) CalcularTotalesPorMetodo(List<Pago> pagos)
        {
            var totalesPorMetodo = pagos
                .GroupBy(p => p.MetodoDePago)
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Total));

            return (
                totalesPorMetodo.GetValueOrDefault(MetodoPago.Efectivo),
                totalesPorMetodo.GetValueOrDefault(MetodoPago.Tarjeta),
                totalesPorMetodo.GetValueOrDefault(MetodoPago.Transferencia),
                totalesPorMetodo.GetValueOrDefault(MetodoPago.MercadoPago)
            );
        }
    }
}
