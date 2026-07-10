using Moq;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using Microsoft.Extensions.Logging;

namespace PanComido.Tests.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class OcuparMesaCasoDeUsoTest
    {
        private readonly Mock<IMesaRepositorio> _mesaMockRepo;
        private readonly Mock<IComandaRepositorio> _comandaMockRepo;
        private readonly Mock<ITurnoFilaRepositorio> _turnoFilaMockRepo;
        private readonly Mock<IMesaNotificador> _mesaNotificadorMock;

        public OcuparMesaCasoDeUsoTest()
        {
            _mesaMockRepo = new Mock<IMesaRepositorio>();
            _comandaMockRepo = new Mock<IComandaRepositorio>();
            _turnoFilaMockRepo = new Mock<ITurnoFilaRepositorio>();
            _mesaNotificadorMock = new Mock<IMesaNotificador>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTodoEsValido_OcupaLaMesaYDevuelveConComanda()
        {
            int restauranteId = 1;
            int mesaId = 1;
            int cantComensales = 2;
            int comandaNuevaId = 10;

            var mesaMock = new MesaConPosiciones
            {
                Id = mesaId,
                EstadoMesa = EstadoMesa.Disponible,
                CantPersonasMax = 4
            };

            _mesaMockRepo
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(mesaMock);

            _mesaMockRepo
                .Setup(r => r.ActualizarEstadoAsync(mesaId, EstadoMesa.Ocupada))
                .Returns(Task.CompletedTask);

            _comandaMockRepo
                .Setup(r => r.CrearAsync(It.IsAny<Comanda>()))
                .ReturnsAsync(comandaNuevaId);

            _mesaNotificadorMock
                .Setup(n => n.NotificarMesaActualizadaAsync(It.IsAny<MesaConPosiciones>(), restauranteId))
                .Returns(Task.CompletedTask);

            var casoDeUso = new OcuparMesaCasoDeUso(_mesaMockRepo.Object, _comandaMockRepo.Object, _turnoFilaMockRepo.Object, _mesaNotificadorMock.Object, new Mock<ILogger<OcuparMesaCasoDeUso>>().Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId, mesaId, cantComensales);

            Assert.NotNull(resultado);
            Assert.Equal(EstadoMesa.Ocupada, resultado.EstadoMesa);
            Assert.Equal(comandaNuevaId, resultado.idComanda);
            
            _comandaMockRepo.Verify(r => r.CrearAsync(It.IsAny<Comanda>()), Times.Once());
            _mesaMockRepo.Verify(r => r.ActualizarEstadoAsync(mesaId, EstadoMesa.Ocupada), Times.Once());
        }

        [Fact]
        public async Task EjecutarAsync_CuandoMesaNoExiste_LanzaArgumentException()
        {
            int restauranteId = 1;
            int mesaId = 1;

            _mesaMockRepo
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync((MesaConPosiciones)null);

            var casoDeUso = new OcuparMesaCasoDeUso(_mesaMockRepo.Object, _comandaMockRepo.Object, _turnoFilaMockRepo.Object, _mesaNotificadorMock.Object, new Mock<ILogger<OcuparMesaCasoDeUso>>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(restauranteId, mesaId, 2));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoMesaNoEstaDisponible_LanzaInvalidOperationException()
        {
            int restauranteId = 1;
            int mesaId = 1;

            _mesaMockRepo
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(new MesaConPosiciones { EstadoMesa = EstadoMesa.Ocupada });

            var casoDeUso = new OcuparMesaCasoDeUso(_mesaMockRepo.Object, _comandaMockRepo.Object, _turnoFilaMockRepo.Object, _mesaNotificadorMock.Object, new Mock<ILogger<OcuparMesaCasoDeUso>>().Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => casoDeUso.EjecutarAsync(restauranteId, mesaId, 2));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoExcedeCapacidad_LanzaInvalidOperationException()
        {
            int restauranteId = 1;
            int mesaId = 1;

            _mesaMockRepo
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(new MesaConPosiciones { EstadoMesa = EstadoMesa.Disponible, CantPersonasMax = 2 });

            var casoDeUso = new OcuparMesaCasoDeUso(_mesaMockRepo.Object, _comandaMockRepo.Object, _turnoFilaMockRepo.Object, _mesaNotificadorMock.Object, new Mock<ILogger<OcuparMesaCasoDeUso>>().Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => casoDeUso.EjecutarAsync(restauranteId, mesaId, 5));
        }

        [Fact]
        public async Task EjecutarAsync_ConTurnoId_AsociaComandaPrecargadaYLaPoneEnEspera()
        {
            int restauranteId = 1;
            int mesaId = 1;
            int cantComensales = 2;
            int turnoId = 12;
            int comandaPreArmadaId = 99;

            var mesaMock = new MesaConPosiciones
            {
                Id = mesaId,
                EstadoMesa = EstadoMesa.Disponible,
                CantPersonasMax = 4
            };

            var turnoMock = new TurnoFila
            {
                Id = turnoId,
                ComandaPreArmadaId = comandaPreArmadaId
            };

            var comandaMock = new Comanda
            {
                Id = comandaPreArmadaId,
                Estado = EstadoComanda.Nueva
            };

            _mesaMockRepo.Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId)).ReturnsAsync(mesaMock);
            _turnoFilaMockRepo.Setup(r => r.ObtenerPorIdAsync(turnoId)).ReturnsAsync(turnoMock);
            _comandaMockRepo.Setup(r => r.ObtenerComandaPorIdAsync(comandaPreArmadaId)).ReturnsAsync(comandaMock);
            
            var casoDeUso = new OcuparMesaCasoDeUso(_mesaMockRepo.Object, _comandaMockRepo.Object, _turnoFilaMockRepo.Object, _mesaNotificadorMock.Object, new Mock<ILogger<OcuparMesaCasoDeUso>>().Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId, mesaId, cantComensales, turnoId);

            Assert.Equal(EstadoMesa.Ocupada, resultado.EstadoMesa);
            Assert.Equal(comandaPreArmadaId, resultado.idComanda);
            Assert.Equal(EstadoComanda.EnEspera, comandaMock.Estado);
            Assert.Equal(mesaId, comandaMock.MesaId);
            
            _comandaMockRepo.Verify(r => r.ActualizarAsync(comandaMock), Times.Once());
            _comandaMockRepo.Verify(r => r.CrearAsync(It.IsAny<Comanda>()), Times.Never());
        }
    }
}
