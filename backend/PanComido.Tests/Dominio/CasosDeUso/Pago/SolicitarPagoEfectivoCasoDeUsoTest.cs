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
        private readonly Mock<ILlamadoNotificador> _llamadoNotificadorMockRepo;

        public SolicitarPagoEfectivoCasoDeUsoTest()
        {
            _comandaMockRepo = new Mock<IComandaRepositorio>();
            _llamadoMockRepo = new Mock<ILlamadoRepositorio>();
            _llamadoNotificadorMockRepo = new Mock<ILlamadoNotificador>();
        }

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
                Estado = EstadoComanda.EnPreparacion
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

            _comandaMockRepo
                .Setup(r => r.ActualizarAsync(It.IsAny<DOM.Comanda>()))
                .Returns(Task.CompletedTask);

            _llamadoMockRepo
                .Setup(r => r.crearLlamadoAsync(It.IsAny<DOM.Llamado>()))
                .ReturnsAsync(llamadoCreado);

            _llamadoNotificadorMockRepo
                .Setup(r => r.NotificarLlamadoAsync(It.IsAny<DOM.Llamado>()))
                .Returns(Task.CompletedTask);

            var casoDeUso = new SolicitarPagoEfectivoCasoDeUso(
                _comandaMockRepo.Object,
                _llamadoMockRepo.Object,
                _llamadoNotificadorMockRepo.Object);

            var resultado = await casoDeUso.EjecutarAsync(comandaId, restauranteId);
            Assert.NotNull(resultado);
            Assert.Equal(7, resultado.CategoriaLlamadoId);
            _llamadoMockRepo.Verify(r => r.crearLlamadoAsync(It.IsAny<DOM.Llamado>()), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaComandaNoExiste_LanzaKeyNotFoundException()
        {
            int comandaId = 1;
            int restauranteId = 1;

            var comanda = new DOM.Comanda
            {
                Id = comandaId,
                RestauranteId = restauranteId,
                MesaId = 1,
                MozoId = 1,
                Estado = EstadoComanda.EnPreparacion
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
                .ReturnsAsync((DOM.Comanda?)null);

            var casoDeUso = new SolicitarPagoEfectivoCasoDeUso(
                _comandaMockRepo.Object,
                _llamadoMockRepo.Object,
                _llamadoNotificadorMockRepo.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(comandaId, restauranteId));
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

            var casoDeUso = new SolicitarPagoEfectivoCasoDeUso(
                _comandaMockRepo.Object,
                _llamadoMockRepo.Object,
                _llamadoNotificadorMockRepo.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(comandaId, restauranteId));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaComandaEstaEnEspera_LanzaArgumentException()
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

            var casoDeUso = new SolicitarPagoEfectivoCasoDeUso(
                _comandaMockRepo.Object,
                _llamadoMockRepo.Object,
                _llamadoNotificadorMockRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(comandaId, restauranteId));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaComandaEstaFinalizada_LanzaArgumentException()
        {
            int comandaId = 1;
            int restauranteId = 1;

            var comanda = new DOM.Comanda
            {
                Id = comandaId,
                RestauranteId = restauranteId,
                MesaId = 1,
                MozoId = 1,
                Estado = EstadoComanda.Finalizada
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

            var casoDeUso = new SolicitarPagoEfectivoCasoDeUso(
                _comandaMockRepo.Object,
                _llamadoMockRepo.Object,
                _llamadoNotificadorMockRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(comandaId, restauranteId));
        }
    }
}
