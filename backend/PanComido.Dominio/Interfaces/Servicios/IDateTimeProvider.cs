using System;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface IDateTimeProvider
    {
        DateTime ObtenerAhora();
        DateTime ObtenerHoy();
    }
}
