using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.PagoCasoDeUso;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Pago
{
    public class SolicitarPagoCasoDeUsoTest
    {
        private readonly Mock<IComandaRepositorio> _comandaMockRepo;
        private readonly Mock<ICrearLlamadoServicio> _crearLlamadoServicioMock;
        private readonly Mock<IVerificarMetodoPagoHabilitadoServicio> _verificarMetodoPagoHabilitadoMockServicio;
        private readonly Mock<ILogger<SolicitarPagoCasoDeUso>> _loggerMock;

        public SolicitarPagoCasoDeUsoTest()
        {
            _comandaMockRepo = new Mock<IComandaRepositorio>();
            _crearLlamadoServicioMock = new Mock<ICrearLlamadoServicio>();
            _verificarMetodoPagoHabilitadoMockServicio = new Mock<IVerificarMetodoPagoHabilitadoServicio>();
            _loggerMock = new Mock<ILogger<SolicitarPagoCasoDeUso>>();

            _verificarMetodoPagoHabilitadoMockServicio
                .Setup(s => s.EstaHabilitadoAsync(It.IsAny<int>(), It.IsAny<MetodoPago>()))
                .ReturnsAsync(true);
        }

        private SolicitarPagoCasoDeUso CrearCasoDeUso() =>
            new SolicitarPagoCasoDeUso(
                _comandaMockRepo.Object,
                _crearLlamadoServicioMock.Object,
                _verificarMetodoPagoHabilitadoMockServicio.Object,
                _loggerMock.Object);

        private static DOM.Comanda ComandaEnEspera(int comandaId, int restauranteId) => new DOM.Comanda
        {
            Id = comandaId,
            RestauranteId = restauranteId,
            MesaId = 1,
            NumeroDeMesa = 5,
            MozoId = 1,
            Estado = EstadoComanda.EnEspera
        };

        [Theory]
        [InlineData(MetodoPago.Efectivo, "efectivo")]
        [InlineData(MetodoPago.Tarjeta, "tarjeta")]
        [InlineData(MetodoPago.Transferencia, "transferencia")]
        public async Task EjecutarAsync_CuandoTodoEsValido_SolicitaElPagoConElMensajeSegunElMetodo(MetodoPago metodoPago, string textoEsperadoEnMensaje)
        {
            int comandaId = 1;
            int restauranteId = 1;
            var comanda = ComandaEnEspera(comandaId, restauranteId);

            var llamadoCreado = new DOM.Llamado
            {
                Id = 1,
                MozoId = 1,
                MesaId = 1,
                CategoriaLlamadoId = (int)CategoriaLlamado.Pago
            };

            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(comanda);

            string? mensajeEnviado = null;
            _crearLlamadoServicioMock
                .Setup(s => s.CrearYNotificarAsync(comanda.MozoId, comanda.MesaId, comanda.NumeroDeMesa, CategoriaLlamado.Pago, It.IsAny<string>()))
                .Callback<int?, int, int, CategoriaLlamado, string?>((_, _, _, _, mensaje) => mensajeEnviado = mensaje)
                .ReturnsAsync(llamadoCreado);

            var resultado = await CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId, metodoPago);

            Assert.NotNull(resultado);
            Assert.Equal((int)CategoriaLlamado.Pago, resultado.CategoriaLlamadoId);
            Assert.NotNull(mensajeEnviado);
            Assert.Contains(textoEsperadoEnMensaje, mensajeEnviado, StringComparison.OrdinalIgnoreCase);
            _crearLlamadoServicioMock.Verify(s => s.CrearYNotificarAsync(comanda.MozoId, comanda.MesaId, comanda.NumeroDeMesa, CategoriaLlamado.Pago, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElMetodoNoEstaHabilitadoParaElRestaurante_LanzaArgumentException()
        {
            int comandaId = 1;
            int restauranteId = 1;
            var comanda = ComandaEnEspera(comandaId, restauranteId);

            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(comanda);

            _verificarMetodoPagoHabilitadoMockServicio
                .Setup(s => s.EstaHabilitadoAsync(restauranteId, MetodoPago.Transferencia))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId, MetodoPago.Transferencia));

            _crearLlamadoServicioMock.Verify(s => s.CrearYNotificarAsync(It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CategoriaLlamado>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElMetodoNoEsSoportado_LanzaArgumentOutOfRangeException()
        {
            int comandaId = 1;
            int restauranteId = 1;
            var comanda = ComandaEnEspera(comandaId, restauranteId);

            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(comanda);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId, MetodoPago.MercadoPago));

            _crearLlamadoServicioMock.Verify(s => s.CrearYNotificarAsync(It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CategoriaLlamado>(), It.IsAny<string>()), Times.Never);
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

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId, MetodoPago.Efectivo));
        }
    }
}
