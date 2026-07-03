using PanComido.Dominio.Entidades.Enums;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface IVerificarMetodoPagoHabilitadoServicio
    {
        Task<bool> EstaHabilitadoAsync(int restauranteId, MetodoPago metodoPago);
    }
}
