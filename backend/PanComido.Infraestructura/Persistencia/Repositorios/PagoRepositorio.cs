using Microsoft.EntityFrameworkCore;
using DOM = PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class PagoRepositorio : IPagoRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly PagoEntityMapper _pagoEntityMapper;

        public PagoRepositorio(AppDbContext ctx, PagoEntityMapper pagoEntityMapper)
        {
            _ctx = ctx;
            _pagoEntityMapper = pagoEntityMapper;
        }

        public async Task<DOM.Pago?> ConfirmarPagoAsync(string externalReference)
        {
            var efPago = await _ctx.Pagos
                .FirstOrDefaultAsync(p => p.ExternalReference == externalReference);

            if (efPago == null) return null;

            efPago.EstadoPagoId = (int)EstadoPago.Confirmado;
            await _ctx.SaveChangesAsync();
            return _pagoEntityMapper.paraDominio(efPago);
        }

        public async Task<Pago?> RechazarPagoAsync(string externalReference)
        {
            var efPago = await _ctx.Pagos
                .FirstOrDefaultAsync(p => p.ExternalReference == externalReference);

            if (efPago == null) return null;

            efPago.EstadoPagoId = (int)EstadoPago.Rechazado;
            await _ctx.SaveChangesAsync();
            return _pagoEntityMapper.paraDominio(efPago);
        }

        public async Task<DOM.Pago> CrearPagoAsync(DOM.Pago pago)
        {
            var efPago = _pagoEntityMapper.paraEntidad(pago);
            await _ctx.Pagos.AddAsync(efPago);
            await _ctx.SaveChangesAsync();

            pago.PagoId = efPago.Id;
            return pago;
        }

        public async Task<DOM.Pago?> ObtenerPagoPorComandaIdAsync(int comandaId)
        {
            var efPago = await _ctx.Pagos
                .FirstOrDefaultAsync(p => p.ComandaId == comandaId);

            return efPago == null ? null : _pagoEntityMapper.paraDominio(efPago);
        }

        public async Task<DOM.Pago?> ObtenerPagoPorExternalReferenceAsync(string externalReference)
        {
            var efPago = await _ctx.Pagos
                .FirstOrDefaultAsync(p => p.ExternalReference == externalReference);

            return efPago == null ? null : _pagoEntityMapper.paraDominio(efPago);
        }

        /*public async Task<List<DOM.Pago>> ObtenerPagosPorFechaAsync(DateTime fecha, int restauranteId)
        {
            var pagos = await _ctx.Pagos
                            .Include(p => p.Comanda)
                            .Where(p => p.Comanda.RestauranteId == restauranteId &&
                                        p.FechaHora.Date == fecha.Date)
                            .ToListAsync();
            return pagos.Select(p => _pagoEntityMapper.paraDominio(p)).ToList();
        }*/

        public async Task<List<DOM.Pago>> ObtenerPagosParaCierreAsync(int restauranteId,
                                                                        DateTime inicio,
                                                                        DateTime fin)
        {
            var pagos = await _ctx.Pagos
                                    .Include(p => p.Comanda)
                                    .Where(p =>
                                        p.CierreId == null &&
                                        p.Comanda.RestauranteId == restauranteId &&
                                        p.FechaHora >= inicio &&
                                        p.FechaHora < fin)
                                    .ToListAsync();

            return pagos
                .Select(p => _pagoEntityMapper.paraDominio(p))
                .ToList();
        }
    }
}
