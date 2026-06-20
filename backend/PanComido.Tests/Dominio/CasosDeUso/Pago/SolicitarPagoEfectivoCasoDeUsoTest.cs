using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.PagoCasoDeUso;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Pago
{
    public class SolicitarPagoEfectivoCasoDeUsoTest
    {
        private readonly Mock<IComandaRepositorio> _comandaMockRepo;
        private readonly Mock<ILlamadoRepositorio> _llamadoMockRepo;
        private readonly Mock<ILlamadoNotificador> _llamadoNotificadorMock;
        private readonly Mock<ILogger<SolicitarPagoEfectivoCasoDeUso>> _loggerMock;

        public SolicitarPagoEfectivoCasoDeUsoTest()
        {
            _comandaMockRepo = new Mock<IComandaRepositorio>();
            _llamadoMockRepo = new Mock<ILlamadoRepositorio>();
            _llamadoNotificadorMock = new Mock<ILlamadoNotificador>();
            _loggerMock = new Mock<ILogger<SolicitarPagoEfectivoCasoDeUso>>();
        }

        private SolicitarPagoEfectivoCasoDeUso CrearCasoDeUso() =>
            new SolicitarPagoEfectivoCasoDeUso(
                _comandaMockRepo.Object,
                _llamadoMockRepo.Object,
                _llamadoNotificadorMock.Object,
                _loggerMock.Object);

        [Fact]
        public async Task EjecutarAsync_CuandoTodoEsValido_SolicitaElPago()
        {
            int comandaId = 1;
            int restauranteId = 1;

            var comanda = new DOM.Comanda
            {
                Id = comandaId,
                RestauranteId = restauranteId,
                MesaId = 1,
                MozoId = 1,
                Estado = EstadoComanda.EnEspera
            };

            var llamadoCreado = new DOM.Llamado
            {
                Id = 1,
                MozoId = 1,
                MesaId = 1,
                CategoriaLlamadoId = 7
            };

            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(comanda);

            _llamadoMockRepo
                .Setup(r => r.crearLlamadoAsync(It.IsAny<DOM.Llamado>()))
                .ReturnsAsync(llamadoCreado);

            _llamadoNotificadorMock
                .Setup(r => r.NotificarLlamadoAsync(It.IsAny<DOM.Llamado>()))
                .Returns(Task.CompletedTask);

            var resultado = await CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId);
            Assert.NotNull(resultado);
            Assert.Equal(7, resultado.CategoriaLlamadoId);
            _llamadoMockRepo.Verify(r => r.crearLlamadoAsync(It.IsAny<DOM.Llamado>()), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaComandaNoExiste_LanzaKeyNotFoundException()
        {
            int comandaId = 1;
            int restauranteId = 1;

            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync((DOM.Comanda?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaComandaEsDeOtroRestaurante_LanzaKeyNotFoundException()
        {
            int comandaId = 1;
            int restauranteId = 1;

            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(new DOM.Comanda
                {
                    Id = comandaId,
                    RestauranteId = 99
                });

            await Assert.ThrowsAsync<KeyNotFoundException>(() => CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaComandaEstaFinalizada_LanzaArgumentException()
        {
            int comandaId = 1;
            int restauranteId = 1;

            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(new DOM.Comanda
                {
                    Id = comandaId,
                    RestauranteId = restauranteId,
                    Estado = EstadoComanda.Finalizada
                });

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId));
        }
    }
}