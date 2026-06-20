using Moq;
using PanComido.Dominio.CasosDeUso.PagoCasoDeUso;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Dominio.Interfaces.Servicios.MercadoPago;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Pago
{
    public class CrearPreferenciaMPCasoDeUsoTest
    {
        private readonly Mock<IComandaRepositorio> _comandaMockRepo;
        private readonly Mock<IMercadoPagoServicio> _mercadoPagoMockServicio;
        private readonly Mock<ICalcularTotalComandaServicio> _calcularTotalMockServicio;
        private readonly Mock<IRestauranteRepositorio> _restauranteMockRepo;
        private readonly Mock<IPagoRepositorio> _pagoMockRepo;

        public CrearPreferenciaMPCasoDeUsoTest()
        {
            _comandaMockRepo = new Mock<IComandaRepositorio>();
            _mercadoPagoMockServicio = new Mock<IMercadoPagoServicio>();
            _calcularTotalMockServicio = new Mock<ICalcularTotalComandaServicio>();
            _restauranteMockRepo = new Mock<IRestauranteRepositorio>();
            _pagoMockRepo = new Mock<IPagoRepositorio>();
        }

        private CrearPreferenciaMPCasoDeUso CrearCasoDeUso() =>
            new CrearPreferenciaMPCasoDeUso(
                _comandaMockRepo.Object,
                _mercadoPagoMockServicio.Object,
                _calcularTotalMockServicio.Object,
                _restauranteMockRepo.Object,
                _pagoMockRepo.Object);

        [Fact]
        public async Task EjecutarAsync_CuandoTodoEsValido_RetornaInitPoint()
        {
            int comandaId = 1;
            int restauranteId = 1;

            var comanda = new DOM.Comanda
            {
                Id = comandaId,
                RestauranteId = restauranteId,
                Estado = EstadoComanda.EnEspera
            };

            var restaurante = new DOM.Restaurante
            {
                Id = restauranteId,
                Nombre = "El Buen Sabor"
            };

            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(comanda);

            _calcularTotalMockServicio
                .Setup(s => s.CalcularTotal(comanda))
                .Returns(1000m);

            _restauranteMockRepo
                .Setup(r => r.ObtenerDatosDelLocalAsync(restauranteId))
                .ReturnsAsync(restaurante);

            _pagoMockRepo
                .Setup(r => r.ObtenerPagoPorComandaIdAsync(comandaId))
                .ReturnsAsync((DOM.Pago?)null);

            _mercadoPagoMockServicio
                .Setup(s => s.CrearPreferenciaAsync("Comanda-1", 1000m, "Pago a El Buen Sabor"))
                .ReturnsAsync("https://mp.com/init-point");

            _pagoMockRepo
                .Setup(r => r.CrearPagoAsync(It.IsAny<DOM.Pago>()))
                .ReturnsAsync(new DOM.Pago());

            var resultado = await CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId);

            Assert.Equal("https://mp.com/init-point", resultado);
            _pagoMockRepo.Verify(r => r.CrearPagoAsync(It.IsAny<DOM.Pago>()), Times.Once);
            _mercadoPagoMockServicio.Verify(s => s.CrearPreferenciaAsync("Comanda-1", 1000m, "Pago a El Buen Sabor"), Times.Once);
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
                .ReturnsAsync(new DOM.Comanda { Id = comandaId, RestauranteId = 99 });

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
                    RestauranteId = restauranteId,
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
                RestauranteId = restauranteId,
                Estado = EstadoComanda.EnEspera
            };

            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(comanda);

            _calcularTotalMockServicio
                .Setup(s => s.CalcularTotal(comanda))
                .Returns(1000m);

            _restauranteMockRepo
                .Setup(r => r.ObtenerDatosDelLocalAsync(restauranteId))
                .ReturnsAsync(new DOM.Restaurante { Id = restauranteId, Nombre = "El Buen Sabor" });

            _pagoMockRepo
                .Setup(r => r.ObtenerPagoPorComandaIdAsync(comandaId))
                .ReturnsAsync(new DOM.Pago { EstadoPago = EstadoPago.Confirmado });

            await Assert.ThrowsAsync<InvalidOperationException>(() => CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId));
        }
    }
}