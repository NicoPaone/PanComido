using PanComido.Dominio.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.Servicios
{
    public class CalculadorVentanaTurnoServicioTest
    {
        private readonly CalculadorVentanaTurnoServicio _servicio = new();

        [Fact]
        public void CalcularVentana_TurnoDiurnoDespuesDeTerminar_DevuelveVentanaDeHoy()
        {
            var turno = new DOM.TurnoLaboral { EsNocturno = false, HorarioInicio = new TimeOnly(10, 0), HorarioFin = new TimeOnly(16, 0) };
            var ahora = new DateTime(2026, 1, 5, 17, 0, 0);

            var (inicio, fin) = _servicio.CalcularVentana(turno, ahora);

            Assert.Equal(new DateTime(2026, 1, 5, 10, 0, 0), inicio);
            Assert.Equal(new DateTime(2026, 1, 5, 16, 0, 0), fin);
        }

        [Fact]
        public void CalcularVentana_TurnoDiurnoEnCurso_LanzaInvalidOperationException()
        {
            var turno = new DOM.TurnoLaboral { EsNocturno = false, HorarioInicio = new TimeOnly(10, 0), HorarioFin = new TimeOnly(16, 0) };
            var ahora = new DateTime(2026, 1, 5, 12, 0, 0);

            Assert.Throws<InvalidOperationException>(() => _servicio.CalcularVentana(turno, ahora));
        }

        [Fact]
        public void CalcularVentana_TurnoDiurnoAntesDeArrancarHoy_DevuelveVentanaDeAyer()
        {
            var turno = new DOM.TurnoLaboral { EsNocturno = false, HorarioInicio = new TimeOnly(10, 0), HorarioFin = new TimeOnly(16, 0) };
            var ahora = new DateTime(2026, 1, 5, 8, 0, 0);

            var (inicio, fin) = _servicio.CalcularVentana(turno, ahora);

            Assert.Equal(new DateTime(2026, 1, 4, 10, 0, 0), inicio);
            Assert.Equal(new DateTime(2026, 1, 4, 16, 0, 0), fin);
        }

        [Fact]
        public void CalcularVentana_TurnoNocturnoDespuesDeMedianoche_DevuelveVentanaDeAnoche()
        {
            var turno = new DOM.TurnoLaboral { EsNocturno = true, HorarioInicio = new TimeOnly(20, 0), HorarioFin = new TimeOnly(1, 0) };
            var ahora = new DateTime(2026, 1, 5, 2, 0, 0);

            var (inicio, fin) = _servicio.CalcularVentana(turno, ahora);

            Assert.Equal(new DateTime(2026, 1, 4, 20, 0, 0), inicio);
            Assert.Equal(new DateTime(2026, 1, 5, 1, 0, 0), fin);
        }

        [Fact]
        public void CalcularVentana_TurnoNocturnoEnCurso_LanzaInvalidOperationException()
        {
            var turno = new DOM.TurnoLaboral { EsNocturno = true, HorarioInicio = new TimeOnly(20, 0), HorarioFin = new TimeOnly(1, 0) };
            var ahora = new DateTime(2026, 1, 5, 22, 0, 0);

            Assert.Throws<InvalidOperationException>(() => _servicio.CalcularVentana(turno, ahora));
        }

        [Fact]
        public void CalcularVentana_TurnoNocturnoJustoAlIniciar_LanzaInvalidOperationException()
        {
            var turno = new DOM.TurnoLaboral { EsNocturno = true, HorarioInicio = new TimeOnly(20, 0), HorarioFin = new TimeOnly(1, 0) };
            var ahora = new DateTime(2026, 1, 5, 20, 0, 0);

            Assert.Throws<InvalidOperationException>(() => _servicio.CalcularVentana(turno, ahora));
        }

        [Fact]
        public void CalcularVentana_TurnoNocturnoLargoTodaviaEnCursoDespuesDeMedianoche_LanzaInvalidOperationException()
        {
            // Turno noche de 16:00 a 06:10 (cruza medianoche). A las 06:08 todavia no llego el horario de fin.
            var turno = new DOM.TurnoLaboral { EsNocturno = true, HorarioInicio = new TimeOnly(16, 0), HorarioFin = new TimeOnly(6, 10) };
            var ahora = new DateTime(2026, 1, 5, 6, 8, 0);

            Assert.Throws<InvalidOperationException>(() => _servicio.CalcularVentana(turno, ahora));
        }

        [Fact]
        public void CalcularVentana_TurnoNocturnoLargoDespuesDeQueTermino_DevuelveVentanaDeAnoche()
        {
            var turno = new DOM.TurnoLaboral { EsNocturno = true, HorarioInicio = new TimeOnly(16, 0), HorarioFin = new TimeOnly(6, 10) };
            var ahora = new DateTime(2026, 1, 5, 6, 20, 0);

            var (inicio, fin) = _servicio.CalcularVentana(turno, ahora);

            Assert.Equal(new DateTime(2026, 1, 4, 16, 0, 0), inicio);
            Assert.Equal(new DateTime(2026, 1, 5, 6, 10, 0), fin);
        }
    }
}
