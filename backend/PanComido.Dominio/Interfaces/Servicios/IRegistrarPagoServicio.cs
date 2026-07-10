using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface IRegistrarPagoServicio
    {
        Task<Pago> RegistrarAsync(int comandaId, decimal total, MetodoPago metodo, EstadoPago estado, string? externalReference = null);
    }
}
