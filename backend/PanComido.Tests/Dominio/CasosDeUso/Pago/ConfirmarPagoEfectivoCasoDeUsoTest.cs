using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.PagoCasoDeUso;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Pago
{
    public class ConfirmarPagoEfectivoCasoDeUsoTest
    {
        private readonly Mock<IPagoRepositorio> _pagoMockRepo;
        private readonly Mock<ILlamadoRepositorio> _llamadoMockRepo;
        private readonly Mock<IComandaRepositorio> _comandaMockRepo;
        private readonly Mock<ICalcularTotalComandaServicio> _calcularTotalMockServicio;
        private readonly Mock<ILogger<ConfirmarPagoEfectivoCasoDeUso>> _loggerMock;

        public ConfirmarPagoEfectivoCasoDeUsoTest()
        {
            _pagoMockRepo = new Mock<IPagoRepositorio>();
            _llamadoMockRepo = new Mock<ILlamadoRepositorio>();
            _comandaMockRepo = new Mock<IComandaRepositorio>();
            _calcularTotalMockServicio = new Mock<ICalcularTotalComandaServicio>();
            _loggerMock = new Mock<ILogger<ConfirmarPagoEfectivoCasoDeUso>>();
        }

        private ConfirmarPagoEfectivoCasoDeUso CrearCasoDeUso() =>
            new ConfirmarPagoEfectivoCasoDeUso(
                _pagoMockRepo.Object,
                _comandaMockRepo.Object,
                _llamadoMockRepo.Object,
                _calcularTotalMockServicio.Object,
                _loggerMock.Object);

        [Fact]
        public async Task EjecutarAsync_CuandoTodoEsValido_ConfirmaElPago()
        {
            int comandaId = 1;
            int restauranteId = 1;

            var comanda = new DOM.Comanda
            {
                Id = comandaId,
                MesaId = 1,
                Estado = EstadoComanda.EnEspera,
                Items = new List<DOM.ArticuloComanda>
                {
                    new DOM.ArticuloComanda
                    {
                        Cantidad = 2,
                        Articulo = new DOM.Plato { PrecioVentaFinal = 500 }
                    }
                }
            };

            var pagoCreado = new DOM.Pago
            {
                PagoId = 1,
                Total = 1000,
                MetodoDePago = MetodoPago.Efectivo
            };

            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(comanda);

            _calcularTotalMockServicio
                .Setup(s => s.CalcularTotal(comanda))
                .Returns(1000m);

            _pagoMockRepo
                .Setup(r => r.ObtenerPagoPorComandaIdAsync(comandaId))
                .ReturnsAsync((DOM.Pago?)null);

            _pagoMockRepo
                .Setup(r => r.CrearPagoAsync(It.IsAny<DOM.Pago>()))
                .ReturnsAsync(pagoCreado);

            _comandaMockRepo
                .Setup(r => r.ActualizarAsync(It.IsAny<DOM.Comanda>()))
                .Returns(Task.CompletedTask);

            _llamadoMockRepo
                .Setup(r => r.ResolverLlamadoPorMesaYCategoriaAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            var resultado = await CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId);
            Assert.NotNull(resultado);
            Assert.Equal(1000, resultado.Total);
            _pagoMockRepo.Verify(r => r.CrearPagoAsync(It.IsAny<DOM.Pago>()), Times.Once);
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
        public async Task EjecutarAsync_CuandoLaComandaNoEstaEnEspera_LanzaArgumentException()
        {
            int comandaId = 1;
            int restauranteId = 1;

            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(new DOM.Comanda
                {
                    Id = comandaId,
                    Estado = EstadoComanda.EnPreparacion
                });

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElPagoYaEstaConfirmado_LanzaInvalidOperationException()
        {
            int comandaId = 1;
            int restauranteId = 1;

            var comanda = new DOM.Comanda
            {
                Id = comandaId,
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

            await Assert.ThrowsAsync<InvalidOperationException>(() => CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId));
        }
    }
}