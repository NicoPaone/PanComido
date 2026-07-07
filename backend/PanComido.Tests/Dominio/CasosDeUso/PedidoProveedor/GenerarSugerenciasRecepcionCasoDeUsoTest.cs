using Moq;
using PanComido.Dominio.CasosDeUso.PedidosCasosDeUso;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.PedidoProveedor
{
    public class GenerarSugerenciasRecepcionCasoDeUsoTest
    {
        private readonly Mock<IPedidoRepositorio> _pedidoRepoMock;
        private readonly Mock<IGeneradorNombreLoteServicio> _generadorNombreLoteServicioMock;
        public GenerarSugerenciasRecepcionCasoDeUsoTest()
        {
            _pedidoRepoMock = new Mock<IPedidoRepositorio>();
            _generadorNombreLoteServicioMock = new Mock<IGeneradorNombreLoteServicio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTodoEsValido_DevuelveSugerencias()
        {
            int pedidoId = 1;
            string timestamp = DateTime.Now.ToString("yyyyMMdd");

            var pedidoExistente = new DOM.Pedido
            {
                Id = pedidoId,
                Estado = EstadoPedidoProveedor.Enviado,
                ItemsInsumo = new List<DOM.PedidoInsumo>
                {
                    new DOM.PedidoInsumo
                    {
                        InsumoId = 10,
                        NombreInsumo = "Tomate",
                        Cantidad = 5,
                        CategoriaInsumoId = 1
                    }
                }
            };

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(pedidoExistente);

            _generadorNombreLoteServicioMock
                .Setup(s => s.GenerarNombreUnicoAsync(It.IsAny<string>()))
                .ReturnsAsync((string nombreInsumo) => $"{nombreInsumo}-{timestamp}");

            var casoDeUso = new GenerarSugerenciasRecepcionCasoDeUso(
                _pedidoRepoMock.Object,
                _generadorNombreLoteServicioMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(pedidoId);
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Count);
            Assert.Equal(10, resultado[0].InsumoId);
            Assert.StartsWith("Tomate-", resultado[0].NombreLote);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElPedidoNoExiste_LanzaKeyNotFoundException()
        {
            int pedidoId = 1;
            string timestamp = DateTime.Now.ToString("yyyyMMdd");

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync((DOM.Pedido?)null);

            var casoDeUso = new GenerarSugerenciasRecepcionCasoDeUso(
                _pedidoRepoMock.Object,
                _generadorNombreLoteServicioMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(pedidoId));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElPedidoNoEstaEnEstadoEnviado_LanzaInvalidOperationException()
        {
            int pedidoId = 1;
            string timestamp = DateTime.Now.ToString("yyyyMMdd");


            var pedidoExistente = new DOM.Pedido
            {
                Id = pedidoId,
                Estado = EstadoPedidoProveedor.Pendiente,
                ItemsInsumo = new List<DOM.PedidoInsumo>
                {
                    new DOM.PedidoInsumo
                    {
                        InsumoId = 10,
                        NombreInsumo = "Tomate",
                        Cantidad = 5,
                        CategoriaInsumoId = 1
                    }
                }
            };

            _pedidoRepoMock
                .Setup(r => r.ObtenerPedidoPorIdAsync(pedidoId))
                .ReturnsAsync(pedidoExistente);

            var casoDeUso = new GenerarSugerenciasRecepcionCasoDeUso(
                _pedidoRepoMock.Object,
                _generadorNombreLoteServicioMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => casoDeUso.EjecutarAsync(pedidoId));
        }
    }
}
