using PanComido.Dominio.Entidades.Dashboard;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface IPoliticaDescuentoDashboardServicio
    {
        Task<PoliticaDescuentoDashboard> ObtenerAsync(int restauranteId);
    }
}
