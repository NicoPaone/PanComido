using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.Servicios
{
    public class VerificarMetodoPagoHabilitadoServicio : IVerificarMetodoPagoHabilitadoServicio
    {
        private readonly IMetodoDePagoRepositorio _metodoDePagoRepositorio;

        public VerificarMetodoPagoHabilitadoServicio(IMetodoDePagoRepositorio metodoDePagoRepositorio)
        {
            _metodoDePagoRepositorio = metodoDePagoRepositorio;
        }

        public async Task<bool> EstaHabilitadoAsync(int restauranteId, MetodoPago metodoPago)
        {
            var metodosDePago = await _metodoDePagoRepositorio.ObtenerMetodosDePagoAsync(restauranteId);
            return metodosDePago.Any(m => m.Id == (int)metodoPago && m.Habilitado);
        }
    }
}
