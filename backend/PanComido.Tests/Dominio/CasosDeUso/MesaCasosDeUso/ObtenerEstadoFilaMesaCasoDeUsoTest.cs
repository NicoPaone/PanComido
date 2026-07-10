using Moq;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso.Resultados;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Tests.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class ObtenerEstadoFilaMesaCasoDeUsoTest
    {
        private readonly Mock<ITurnoFilaRepositorio> _turnoMockRepo;
        private readonly Mock<IMesaRepositorio> _mesaMockRepo;
        private readonly Mock<ICacheServicio> _cacheMock;

        public ObtenerEstadoFilaMesaCasoDeUsoTest()
        {
            _turnoMockRepo = new Mock<ITurnoFilaRepositorio>();
            _mesaMockRepo = new Mock<IMesaRepositorio>();
            _cacheMock = new Mock<ICacheServicio>();
        }

        [Fact]
        public async Task EjecutarAsync_DevuelveDesdeCache_SiExiste()
        {
            int turnoId = 1;
            var resultadoCache = new EstadoFilaMesaResult { TiempoEstimadoMinutos = 10 };
            
            _cacheMock.Setup(c => c.TryGetValue($"EstadoFilaMesa_{turnoId}", out resultadoCache)).Returns(true);

            var casoDeUso = new ObtenerEstadoFilaMesaCasoDeUso(_turnoMockRepo.Object, _mesaMockRepo.Object, _cacheMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(turnoId);

            Assert.Equal(10, resultado.TiempoEstimadoMinutos);
            _turnoMockRepo.Verify(r => r.ObtenerPorIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_MesaAsignada_DevuelveMesaLista()
        {
            int turnoId = 1;
            var turno = new TurnoFila
            {
                Id = turnoId,
                Estado = EstadoTurnoMesa.MesaAsignada,
                FechaHoraAsignacion = DateTime.UtcNow.AddMinutes(-2),
                MesaAsignadaId = 5
            };

            EstadoFilaMesaResult cacheOut = null;
            _cacheMock.Setup(c => c.TryGetValue(It.IsAny<string>(), out cacheOut)).Returns(false);
            _turnoMockRepo.Setup(r => r.ObtenerPorIdAsync(turnoId)).ReturnsAsync(turno);

            var casoDeUso = new ObtenerEstadoFilaMesaCasoDeUso(_turnoMockRepo.Object, _mesaMockRepo.Object, _cacheMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(turnoId);

            Assert.True(resultado.MesaLista);
            Assert.Equal(5, resultado.MesaAsignadaId);
            Assert.Equal("¡Tu mesa está lista!", resultado.TiempoEstimadoVisual);
        }

        [Fact]
        public async Task EjecutarAsync_CalculaTiempoYDevuelveVisualCorrecto()
        {
            int turnoId = 1;
            var turno = new TurnoFila
            {
                Id = turnoId,
                FilaVirtualId = 1,
                CantidadComensales = 2,
                FechaHoraIngreso = DateTime.UtcNow,
                Estado = EstadoTurnoMesa.EnEspera
            };

            var filaVirtual = new FilaVirtual { Id = 1, RestauranteId = 1, TiempoPromedioComidaMinutos = 40 };

            var mesas = new List<MesaFilaVirtualDto>
            {
                new MesaFilaVirtualDto { Id = 1, CantPersonasMax = 4, EstadoMesa = EstadoMesa.Disponible }
            };

            EstadoFilaMesaResult cacheOut = null;
            _cacheMock.Setup(c => c.TryGetValue(It.IsAny<string>(), out cacheOut)).Returns(false);
            _turnoMockRepo.Setup(r => r.ObtenerPorIdAsync(turnoId)).ReturnsAsync(turno);
            _turnoMockRepo.Setup(r => r.ContarTurnosEnEsperaPreviosAsync(1, It.IsAny<DateTime>())).ReturnsAsync(0);
            _turnoMockRepo.Setup(r => r.ObtenerFilaVirtualPorIdAsync(1)).ReturnsAsync(filaVirtual);
            _mesaMockRepo.Setup(r => r.ObtenerMesasParaFilaVirtualAsync(1)).ReturnsAsync(mesas);

            var casoDeUso = new ObtenerEstadoFilaMesaCasoDeUso(_turnoMockRepo.Object, _mesaMockRepo.Object, _cacheMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(turnoId);

            Assert.False(resultado.MesaLista);
            Assert.Equal(0, resultado.TurnosAdelante);
            // Al estar la mesa disponible y haber 0 turnos adelante, el tiempo es 0.
            Assert.Equal(0, resultado.TiempoEstimadoMinutos);
            Assert.Equal("0 minutos", resultado.TiempoEstimadoVisual);
            
            _cacheMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<EstadoFilaMesaResult>(), It.IsAny<TimeSpan>()), Times.Once);
        }
    }
}
