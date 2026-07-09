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

            var inicioAyer = inicioHoy.AddDays(-1);
            var finAyer = finHoy.AddDays(-1);

            bool enCursoHoy = ahora >= inicioHoy && ahora < finHoy;
            bool enCursoAyer = ahora >= inicioAyer && ahora < finAyer;

            if (enCursoHoy || enCursoAyer)
                throw new InvalidOperationException("No se puede cerrar un turno que todavía está en curso.");

            if (ahora < inicioHoy)
                return (inicioAyer, finAyer);

            return (inicioHoy, finHoy);
        }
    }
}
