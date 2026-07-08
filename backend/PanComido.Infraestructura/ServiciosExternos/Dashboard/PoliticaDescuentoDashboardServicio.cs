using Microsoft.Extensions.Options;
using PanComido.Dominio.Entidades.Dashboard;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Infraestructura.ServiciosExternos.Dashboard
{
    public class PoliticaDescuentoDashboardServicio : IPoliticaDescuentoDashboardServicio
    {
        private readonly PoliticaDescuentoDashboardConfiguracion _configuracion;

        public PoliticaDescuentoDashboardServicio(IOptions<PoliticaDescuentoDashboardConfiguracion> configuracion)
        {
            _configuracion = configuracion.Value;
        }

        public Task<PoliticaDescuentoDashboard> ObtenerAsync(int restauranteId)
        {
            return Task.FromResult(new PoliticaDescuentoDashboard
            {
                PorcentajeDescuentoMaximo = _configuracion.PorcentajeDescuentoMaximo,
                MargenMinimoPermitido = _configuracion.MargenMinimoPermitido
            });
        }
    }
}
