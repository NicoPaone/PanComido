using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.Servicios
{
    public class CalculadorVentanaTurnoServicio : ICalculadorVentanaTurnoServicio
    {
        public (DateTime Inicio, DateTime Fin) CalcularVentana(TurnoLaboral turno, DateTime ahora)
        {
            bool cruzaMedianoche = turno.HorarioFin <= turno.HorarioInicio;

            var inicioHoy = ahora.Date + turno.HorarioInicio.ToTimeSpan();
            var finHoy = ahora.Date + turno.HorarioFin.ToTimeSpan();
            if (cruzaMedianoche)
                finHoy = finHoy.AddDays(1);

            if (ahora >= inicioHoy && ahora < finHoy)
                throw new InvalidOperationException("No se puede cerrar un turno que todavía está en curso.");

            if (ahora < inicioHoy)
                return (inicioHoy.AddDays(-1), finHoy.AddDays(-1));

            return (inicioHoy, finHoy);
        }
    }
}
