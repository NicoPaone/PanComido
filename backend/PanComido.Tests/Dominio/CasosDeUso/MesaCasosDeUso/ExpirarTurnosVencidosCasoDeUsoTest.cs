using Moq;
using Microsoft.Extensions.Logging;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Tests.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class ExpirarTurnosVencidosCasoDeUsoTest
    {
        private readonly Mock<ITurnoFilaRepositorio> _turnoFilaRepositorioMock;
        private readonly Mock<IMesaRepositorio> _mesaRepositorioMock;
        private readonly Mock<IFilaVirtualNotificador> _filaVirtualNotificadorMock;
        private readonly Mock<ILogger<ExpirarTurnosVencidosCasoDeUso>> _loggerMock;
        private readonly Mock<IMesaNotificador> _mesaNotificadorMock;

        public ExpirarTurnosVencidosCasoDeUsoTest()
        {
            _turnoFilaRepositorioMock = new Mock<ITurnoFilaRepositorio>();
            _mesaRepositorioMock = new Mock<IMesaRepositorio>();
            _filaVirtualNotificadorMock = new Mock<IFilaVirtualNotificador>();
            _loggerMock = new Mock<ILogger<ExpirarTurnosVencidosCasoDeUso>>();
            _mesaNotificadorMock = new Mock<IMesaNotificador>();
        }

        [Fact]
        public async Task EjecutarAsync_ExpiraTurnoViejo_YNotifica_YAsignaAlSiguiente()
        {
            // Arrange
            int restauranteId = 1;
            int mesaId = 10;
            int turnoExpiradoId = 5;
            int siguienteTurnoId = 6;
            
            var turnoExpirado = new TurnoFila
            {
                Id = turnoExpiradoId,
                FilaVirtualId = 1,
                MesaAsignadaId = mesaId,
                Estado = EstadoTurnoMesa.MesaAsignada,
                FechaHoraAsignacion = DateTime.UtcNow.AddMinutes(-10) // 10 min, supera los 7
            };

            var turnosExpirados = new List<TurnoFila> { turnoExpirado };

            var mesa = new MesaConPosiciones
            {
                Id = mesaId,
                CantPersonasMax = 4,
                EstadoMesa = EstadoMesa.Reservada
            };

            var turnoSiguiente = new TurnoFila
            {
                Id = siguienteTurnoId,
                FilaVirtualId = 1,
                CantidadComensales = 3,
                Estado = EstadoTurnoMesa.EnEspera
            };

            _turnoFilaRepositorioMock
                .Setup(r => r.ObtenerTurnosAsignadosExpiradosAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(turnosExpirados);

            _turnoFilaRepositorioMock
                .Setup(r => r.ObtenerFilaVirtualPorIdAsync(1))
                .ReturnsAsync(new FilaVirtual { Id = 1, RestauranteId = restauranteId });

            _mesaRepositorioMock
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(mesa);

            _turnoFilaRepositorioMock
                .Setup(r => r.ObtenerProximoTurnoEnEsperaAsync(1, 4))
                .ReturnsAsync(turnoSiguiente);

            var casoDeUso = new ExpirarTurnosVencidosCasoDeUso(
                _turnoFilaRepositorioMock.Object,
                _mesaRepositorioMock.Object,
                _filaVirtualNotificadorMock.Object,
                _mesaNotificadorMock.Object);

            // Act
            await casoDeUso.EjecutarAsync();

            // Assert
            Assert.Equal(EstadoTurnoMesa.Cancelado, turnoExpirado.Estado);
            _turnoFilaRepositorioMock.Verify(r => r.ActualizarAsync(turnoExpirado), Times.Once);
            _filaVirtualNotificadorMock.Verify(n => n.NotificarTurnoExpiradoAsync(turnoExpiradoId, It.IsAny<string>()), Times.Once);

            Assert.Equal(EstadoTurnoMesa.MesaAsignada, turnoSiguiente.Estado);
            Assert.Equal(mesaId, turnoSiguiente.MesaAsignadaId);
            Assert.NotNull(turnoSiguiente.FechaHoraAsignacion);
            _turnoFilaRepositorioMock.Verify(r => r.ActualizarAsync(turnoSiguiente), Times.Once);
            _filaVirtualNotificadorMock.Verify(n => n.NotificarMesaListaAsync(siguienteTurnoId, mesaId, It.IsAny<int>()), Times.Once);
        }
        
        [Fact]
        public async Task EjecutarAsync_SinSiguienteTurno_LiberaLaMesa()
        {
            // Arrange
            int restauranteId = 1;
            int mesaId = 10;
            
            var turnoExpirado = new TurnoFila
            {
                Id = 1,
                FilaVirtualId = 1,
                MesaAsignadaId = mesaId,
                Estado = EstadoTurnoMesa.MesaAsignada,
                FechaHoraAsignacion = DateTime.UtcNow.AddMinutes(-8)
            };

            _turnoFilaRepositorioMock
                .Setup(r => r.ObtenerTurnosAsignadosExpiradosAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<TurnoFila> { turnoExpirado });

            _turnoFilaRepositorioMock
                .Setup(r => r.ObtenerFilaVirtualPorIdAsync(1))
                .ReturnsAsync(new FilaVirtual { Id = 1, RestauranteId = restauranteId });

            _mesaRepositorioMock
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(new MesaConPosiciones { Id = mesaId, CantPersonasMax = 4, EstadoMesa = EstadoMesa.Reservada });

            _turnoFilaRepositorioMock
                .Setup(r => r.ObtenerProximoTurnoEnEsperaAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((TurnoFila)null); // No hay nadie más

            var casoDeUso = new ExpirarTurnosVencidosCasoDeUso(
                _turnoFilaRepositorioMock.Object,
                _mesaRepositorioMock.Object,
                _filaVirtualNotificadorMock.Object,
                _mesaNotificadorMock.Object);

            // Act
            await casoDeUso.EjecutarAsync();

            // Assert
            _mesaRepositorioMock.Verify(r => r.ActualizarEstadoAsync(mesaId, EstadoMesa.Disponible), Times.Once);
        }
    }
}
