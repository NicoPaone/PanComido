using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Dominio.CasosDeUso.CierreCajaCasoDeUso
{
    public class ObtenerDetalleCierreCasoDeUso
    {
        private readonly IPagoRepositorio _pagoRepositorio;
        private readonly ITurnoLaboralRepositorio _turnoLaboralRepositorio;

        public ObtenerDetalleCierreCasoDeUso(IPagoRepositorio pagoRepositorio, ITurnoLaboralRepositorio turnoLaboralRepositorio)
        {
            _pagoRepositorio = pagoRepositorio;
            _turnoLaboralRepositorio = turnoLaboralRepositorio;
        }

        public async Task<(TurnoLaboral Turno, int CantidadTotalDePagos, decimal TotalRecaudado, List<ResumenMetodoPago> ResumenPorMetodo)> EjecutarAsync(int restauranteId, Cierre cierre)
        {
            var pagos = await _pagoRepositorio.ObtenerPagosPorCierreIdAsync(cierre.CierreId);

            var turnos = await _turnoLaboralRepositorio.ObtenerTurnosLaboralesAsync(restauranteId);
            var turno = turnos.FirstOrDefault(t => t.Id == cierre.TurnoLaboralId);
            if (turno == null) throw new KeyNotFoundException("Turno no encontrado.");

            var totalPorMetodo = new Dictionary<MetodoPago, decimal>
            {
                [MetodoPago.Efectivo] = cierre.TotalEfectivo,
                [MetodoPago.Tarjeta] = cierre.TotalTarjeta,
                [MetodoPago.Transferencia] = cierre.TotalTransferencia,
                [MetodoPago.MercadoPago] = cierre.TotalMercadoPago
            };

            var resumenPorMetodo = pagos
                .GroupBy(p => p.MetodoDePago)
                .Select(g => new ResumenMetodoPago(g.Key, g.Count(), totalPorMetodo.GetValueOrDefault(g.Key)))
                .ToList();

            decimal totalRecaudado = cierre.TotalEfectivo + cierre.TotalTarjeta + cierre.TotalTransferencia + cierre.TotalMercadoPago;

            return (turno, pagos.Count, totalRecaudado, resumenPorMetodo);
        }
    }
}
