using PanComido.Dominio.Interfaces.Servicios;
using System;

namespace PanComido.Dominio.Servicios
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime ObtenerAhora() => DateTime.Now;
        public DateTime ObtenerHoy() => DateTime.Today;
    }
}
