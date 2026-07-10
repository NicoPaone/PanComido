using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.PagoCasoDeUso;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Pago
{
    public class ConfirmarPagoCasoDeUsoTest
    {
        private readonly Mock<IPagoRepositorio> _pagoMockRepo;
        private readonly Mock<ILlamadoRepositorio> _llamadoMockRepo;
        private readonly Mock<IComandaRepositorio> _comandaMockRepo;
        private readonly Mock<ICalcularTotalComandaServicio> _calcularTotalMockServicio;
        private readonly Mock<IComandaNotificador> _comandaNotificadorMock;
        private readonly Mock<ILlamadoNotificador> _llamadoNotificadorMock;
        private readonly Mock<IRegistrarPagoServicio> _registrarPagoMockServicio;
        private readonly Mock<IVerificarMetodoPagoHabilitadoServicio> _verificarMetodoPagoHabilitadoMockServicio;
        private readonly Mock<ILogger<ConfirmarPagoCasoDeUso>> _loggerMock;

        public ConfirmarPagoCasoDeUsoTest()
        {
            _pagoMockRepo = new Mock<IPagoRepositorio>();
            _llamadoMockRepo = new Mock<ILlamadoRepositorio>();
            _comandaMockRepo = new Mock<IComandaRepositorio>();
            _calcularTotalMockServicio = new Mock<ICalcularTotalComandaServicio>();
            _comandaNotificadorMock = new Mock<IComandaNotificador>();
            _llamadoNotificadorMock = new Mock<ILlamadoNotificador>();
            _registrarPagoMockServicio = new Mock<IRegistrarPagoServicio>();
            _verificarMetodoPagoHabilitadoMockServicio = new Mock<IVerificarMetodoPagoHabilitadoServicio>();
            _loggerMock = new Mock<ILogger<ConfirmarPagoCasoDeUso>>();

            _verificarMetodoPagoHabilitadoMockServicio
                .Setup(s => s.EstaHabilitadoAsync(It.IsAny<int>(), It.IsAny<MetodoPago>()))
                .ReturnsAsync(true);
        }

        private ConfirmarPagoCasoDeUso CrearCasoDeUso() =>
            new ConfirmarPagoCasoDeUso(
                _pagoMockRepo.Object,
                _comandaMockRepo.Object,
                _llamadoMockRepo.Object,
                _calcularTotalMockServicio.Object,
                _comandaNotificadorMock.Object,
                _llamadoNotificadorMock.Object,
                _registrarPagoMockServicio.Object,
                _verificarMetodoPagoHabilitadoMockServicio.Object,
                _loggerMock.Object);

        private void ConfigurarFlujoValido(DOM.Comanda comanda, DOM.Pago pagoCreado)
        {
            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(comanda.Id))
                .ReturnsAsync(comanda);

            _calcularTotalMockServicio
                .Setup(s => s.CalcularTotal(comanda))
                .Returns(pagoCreado.Total);

            _pagoMockRepo
                .Setup(r => r.ObtenerPagoPorComandaIdAsync(comanda.Id))
                .ReturnsAsync((DOM.Pago?)null);

            _registrarPagoMockServicio
                .Setup(s => s.RegistrarAsync(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<MetodoPago>(), It.IsAny<EstadoPago>(), null))
                .ReturnsAsync(pagoCreado);

            _comandaMockRepo
                .Setup(r => r.ActualizarAsync(It.IsAny<DOM.Comanda>()))
                .Returns(Task.CompletedTask);

            _llamadoMockRepo
                .Setup(r => r.ResolverLlamadoPorMesaYCategoriaAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((DOM.Llamado?)null);

            _comandaNotificadorMock
                .Setup(n => n.NotificarEstadoModificadoAsync(It.IsAny<DOM.Comanda>(), It.IsAny<List<int>>()))
                .Returns(Task.CompletedTask);
        }

        [Theory]
        [InlineData(MetodoPago.Efectivo)]
        [InlineData(MetodoPago.Tarjeta)]
        [InlineData(MetodoPago.Transferencia)]
        public async Task EjecutarAsync_CuandoTodoEsValido_ConfirmaElPagoConElMetodoIndicado(MetodoPago metodoPago)
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

            var pagoCreado = new DOM.Pago
            {
                PagoId = 1,
                Total = 1000,
                MetodoDePago = metodoPago
            };

            ConfigurarFlujoValido(comanda, pagoCreado);

            var resultado = await CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId, metodoPago);

            Assert.NotNull(resultado);
            Assert.Equal(1000, resultado.Total);
            _registrarPagoMockServicio.Verify(s => s.RegistrarAsync(comandaId, It.IsAny<decimal>(), metodoPago, EstadoPago.Confirmado, null), Times.Once);
            _comandaNotificadorMock.Verify(n => n.NotificarEstadoModificadoAsync(It.IsAny<DOM.Comanda>(), It.IsAny<List<int>>()), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElMetodoEsMercadoPago_LanzaArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(1, 1, MetodoPago.MercadoPago));

            _comandaMockRepo.Verify(r => r.ObtenerComandaPorIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElMetodoNoEstaHabilitadoParaElRestaurante_LanzaArgumentException()
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

            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(comanda);

            _pagoMockRepo
                .Setup(r => r.ObtenerPagoPorComandaIdAsync(comandaId))
                .ReturnsAsync((DOM.Pago?)null);

            _verificarMetodoPagoHabilitadoMockServicio
                .Setup(s => s.EstaHabilitadoAsync(restauranteId, MetodoPago.Transferencia))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId, MetodoPago.Transferencia));

            _registrarPagoMockServicio.Verify(s => s.RegistrarAsync(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<MetodoPago>(), It.IsAny<EstadoPago>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaComandaNoExiste_LanzaKeyNotFoundException()
        {
            int comandaId = 1;
            int restauranteId = 1;

            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync((DOM.Comanda?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId, MetodoPago.Efectivo));
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

            await Assert.ThrowsAsync<KeyNotFoundException>(() => CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId, MetodoPago.Efectivo));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaComandaNoEstaEnEspera_LanzaArgumentException()
        {
            int comandaId = 1;
            int restauranteId = 1;

            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(new DOM.Comanda
                {
                    Id = comandaId,
                    RestauranteId = restauranteId,
                    Estado = EstadoComanda.EnPreparacion
                });

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId, MetodoPago.Efectivo));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElPagoYaEstaConfirmado_LanzaInvalidOperationException()
        {
            int comandaId = 1;
            int restauranteId = 1;

            var comanda = new DOM.Comanda
            {
                Id = comandaId,
                RestauranteId = restauranteId,
                MesaId = 1,
                Estado = EstadoComanda.EnEspera
            };

            var pagoExistente = new DOM.Pago
            {
                PagoId = 1,
                ComandaId = comandaId,
                EstadoPago = EstadoPago.Confirmado
            };

            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(comanda);

            _calcularTotalMockServicio
                .Setup(s => s.CalcularTotal(comanda))
                .Returns(1000m);

            _pagoMockRepo
                .Setup(r => r.ObtenerPagoPorComandaIdAsync(comandaId))
                .ReturnsAsync(pagoExistente);

            await Assert.ThrowsAsync<InvalidOperationException>(() => CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId, MetodoPago.Efectivo));
        }
    }
}
