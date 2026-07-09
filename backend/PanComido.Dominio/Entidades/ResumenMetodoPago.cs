using PanComido.Dominio.Entidades.Enums;

namespace PanComido.Dominio.Entidades
{
    public record ResumenMetodoPago(MetodoPago Metodo, int CantidadPagos, decimal Total);
}
