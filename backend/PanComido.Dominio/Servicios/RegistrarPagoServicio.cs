using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.Servicios
{
    public class RegistrarPagoServicio : IRegistrarPagoServicio
    {
        private readonly IPagoRepositorio _pagoRepositorio;

        public RegistrarPagoServicio(IPagoRepositorio pagoRepositorio)
        {
            _pagoRepositorio = pagoRepositorio;
        }
        public async Task<Pago> RegistrarAsync(int comandaId, decimal total, MetodoPago metodo, EstadoPago estado, string? externalReference = null)
        {
            var pago = new Pago
            {
                ComandaId = comandaId,
                Total = total,
                MetodoDePago = metodo,
                EstadoPago = estado,
                ExternalReference = externalReference,
                FechaHora = DateTime.Now
            };
            return await _pagoRepositorio.CrearPagoAsync(pago);
        }
    }
}
