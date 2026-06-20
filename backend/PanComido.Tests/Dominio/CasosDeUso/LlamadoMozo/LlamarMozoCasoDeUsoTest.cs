using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.LlamadoMozoCasoDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.LlamadoMozo
{
    public class LlamarMozoCasoDeUsoTest
    {
        private readonly Mock<IMozoRepositorio> _mozoRepoMock;
        private readonly Mock<ILlamadoRepositorio> _llamadoRepoMock;
        private readonly Mock<ILlamadoNotificador> _llamadoNotificadorMock;
        private readonly Mock<IMesaRepositorio> _mesaRepoMock;
        private readonly Mock<ILogger<LlamarMozoCasoDeUso>> _loggerMock;

        public LlamarMozoCasoDeUsoTest()
        {
            _mozoRepoMock = new Mock<IMozoRepositorio>();
            _llamadoRepoMock = new Mock<ILlamadoRepositorio>();
            _llamadoNotificadorMock = new Mock<ILlamadoNotificador>();
            _mesaRepoMock = new Mock<IMesaRepositorio>();
            _loggerMock = new Mock<ILogger<LlamarMozoCasoDeUso>>();
        }

        private LlamarMozoCasoDeUso CrearCasoDeUso() =>
            new LlamarMozoCasoDeUso(
                _mozoRepoMock.Object,
                _llamadoRepoMock.Object,
                _llamadoNotificadorMock.Object,
                _mesaRepoMock.Object,
                _loggerMock.Object);

        [Fact]
        public async Task EjecutarAsync_CuandoTodoEsValido_CreaElLlamado()
        {
            int restauranteId = 1;
            int mesaId = 1;
            int categoriaLlamadoId = 2;
            string descripcion = "Necesito sal";

            var llamadoGuardado = new DOM.Llamado { Id = 1, MozoId = 1, MesaId = mesaId };

            _mozoRepoMock
                .Setup(r => r.ObtenerMozoAsignadoAMesaAsync(mesaId))
                .ReturnsAsync(1);

            _mesaRepoMock
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(new DOM.MesaConPosiciones { Id = mesaId });

            _llamadoRepoMock
                .Setup(r => r.crearLlamadoAsync(It.IsAny<DOM.Llamado>()))
                .ReturnsAsync(llamadoGuardado);

            _llamadoNotificadorMock
                .Setup(r => r.NotificarLlamadoAsync(It.IsAny<DOM.Llamado>()))
                .Returns(Task.CompletedTask);

            var resultado = await CrearCasoDeUso().EjecutarAsync(restauranteId, mesaId, categoriaLlamadoId, descripcion);

            Assert.NotNull(resultado);
            _llamadoRepoMock.Verify(r => r.crearLlamadoAsync(It.IsAny<DOM.Llamado>()), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoHayMozoAsignado_LanzaKeyNotFoundException()
        {
            int restauranteId = 1;
            int mesaId = 1;

            _mozoRepoMock
                .Setup(r => r.ObtenerMozoAsignadoAMesaAsync(mesaId))
                .ReturnsAsync(0);

            _mesaRepoMock
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(new DOM.MesaConPosiciones { Id = mesaId });

            await Assert.ThrowsAsync<KeyNotFoundException>(() => CrearCasoDeUso().EjecutarAsync(restauranteId, mesaId, 1, ""));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaMesaNoExiste_LanzaKeyNotFoundException()
        {
            int restauranteId = 1;
            int mesaId = 1;

            _mozoRepoMock
                .Setup(r => r.ObtenerMozoAsignadoAMesaAsync(mesaId))
                .ReturnsAsync(1);

            _mesaRepoMock
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync((DOM.MesaConPosiciones?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => CrearCasoDeUso().EjecutarAsync(restauranteId, mesaId, 4, ""));
        }
    }
}