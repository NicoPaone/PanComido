using Moq;
using PanComido.Dominio.CasosDeUso.PedidosCasosDeUso;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Infraestructura.Persistencia.Entidades;
using DOM = PanComido.Dominio.Entidades;


namespace PanComido.Tests.Dominio.CasosDeUso.Comandas
{
    public class MarcarItemsEntregadosCasoDeUsoTest
    {
        private readonly Mock<IComandaRepositorio> _comandaRepoMock;
        private readonly Mock<IComandaNotificador> _comandaNotificadorMock;
        private readonly Mock<IMesaRepositorio> _mesaRepoMock;

        public MarcarItemsEntregadosCasoDeUsoTest()
        {
            _comandaRepoMock = new Mock<IComandaRepositorio>();
            _comandaNotificadorMock = new Mock<IComandaNotificador>();
            _mesaRepoMock = new Mock<IMesaRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTodoEsValido_MarcaItemsYDevuelveComanda()
        {
            int comandaId = 1;
            int mesaId = 1;
            var articuloComandaIds = new List<int> { 10 };

            var comandaInicial = new DOM.Comanda
            {
                Id = comandaId,
                MesaId = mesaId,
                Estado = EstadoComanda.EnPreparacion,
                Items = new List<DOM.ArticuloComanda>
                {
                    new DOM.ArticuloComanda { Id = 10, Entregado = false }
                }
            };

            var comandaDespuesDeEntregados = new DOM.Comanda
            {
                Id = comandaId,
                MesaId = mesaId,
                Estado = EstadoComanda.EnPreparacion,
                Items = new List<DOM.ArticuloComanda>
                {
                    new DOM.ArticuloComanda { Id = 10, Entregado = true }
                }
            };

            _comandaRepoMock
                .SetupSequence(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(comandaInicial)
                .ReturnsAsync(comandaDespuesDeEntregados)
                .ReturnsAsync(comandaDespuesDeEntregados);

            _comandaRepoMock
                .Setup(r => r.MarcarItemsEntregadosAsync(comandaId, articuloComandaIds))
                .Returns(Task.CompletedTask);

            _comandaRepoMock
                .Setup(r => r.ModificarEstadoComandaAsync(comandaId, It.IsAny<int>()))
                .ReturnsAsync(new DOM.Comanda());

            _mesaRepoMock
                .Setup(r => r.ObtenerMozoIdsPorMesaAsync(mesaId))
                .ReturnsAsync(new List<int> { 1 });

            _comandaNotificadorMock
                .Setup(r => r.NotificarEstadoModificadoAsync(It.IsAny<DOM.Comanda>(), It.IsAny<List<int>>()))
                .Returns(Task.CompletedTask);

            var casoDeUso = new MarcarItemsEntregadosCasoDeUso(
                _comandaRepoMock.Object,
                _comandaNotificadorMock.Object,
                _mesaRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(comandaId, articuloComandaIds);
            Assert.NotNull(resultado);
            Assert.Equal(comandaId, resultado.Id);
            _comandaRepoMock.Verify(r => r.MarcarItemsEntregadosAsync(comandaId, articuloComandaIds), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaComandaNoExiste_LanzaKeyNotFoundException()
        {
            int comandaId = 1;
            var articuloComandaIds = new List<int> { 10 };

            _comandaRepoMock
                .SetupSequence(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync((DOM.Comanda?)null);

            var casoDeUso = new MarcarItemsEntregadosCasoDeUso(
                _comandaRepoMock.Object,
                _comandaNotificadorMock.Object,
                _mesaRepoMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(comandaId, articuloComandaIds));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaComandaEstaFinalizada_LanzaInvalidOperationException()
        {
            int comandaId = 1;
            var articuloComandaIds = new List<int> { 10 };

            _comandaRepoMock
                .Setup(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(new DOM.Comanda
                {
                    Id = comandaId,
                    Estado = EstadoComanda.Finalizada,
                    Items = new List<DOM.ArticuloComanda>()
                });

            var casoDeUso = new MarcarItemsEntregadosCasoDeUso(
                _comandaRepoMock.Object,
                _comandaNotificadorMock.Object,
                _mesaRepoMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => casoDeUso.EjecutarAsync(comandaId, articuloComandaIds));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElItemNoExisteEnLaComanda_LanzaKeyNotFoundException()
        {
            int comandaId = 1;
            var articuloComandaIds = new List<int> { 99 };

            _comandaRepoMock
                .Setup(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(new DOM.Comanda
                {
                    Id = comandaId,
                    Estado = EstadoComanda.EnPreparacion,
                    Items = new List<DOM.ArticuloComanda>
                    {
            new DOM.ArticuloComanda { Id = 10, Entregado = false }
                    }
                });

            var casoDeUso = new MarcarItemsEntregadosCasoDeUso(
                _comandaRepoMock.Object,
                _comandaNotificadorMock.Object,
                _mesaRepoMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(comandaId, articuloComandaIds));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElItemYaFueEntregado_LanzaInvalidOperationException()
        {
            int comandaId = 1;
            int mesaId = 1;
            var articuloComandaIds = new List<int> { 10 };

            var comandaInicial = new DOM.Comanda
            {
                Id = comandaId,
                MesaId = mesaId,
                Estado = EstadoComanda.EnPreparacion,
                Items = new List<DOM.ArticuloComanda>
                {
                    new DOM.ArticuloComanda { Id = 10, Entregado = true }
                }
            };

            _comandaRepoMock
                .Setup(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(comandaInicial);

            var casoDeUso = new MarcarItemsEntregadosCasoDeUso(
                _comandaRepoMock.Object,
                _comandaNotificadorMock.Object,
                _mesaRepoMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => casoDeUso.EjecutarAsync(comandaId, articuloComandaIds));
        }
    }
}
