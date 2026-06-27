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
        private readonly Mock<ICrearLlamadoServicio> _crearLlamadoServicioMock;
        private readonly Mock<ILogger<SolicitarPagoEfectivoCasoDeUso>> _loggerMock;

        public SolicitarPagoEfectivoCasoDeUsoTest()
        {
            _comandaMockRepo = new Mock<IComandaRepositorio>();
            _crearLlamadoServicioMock = new Mock<ICrearLlamadoServicio>();
            _loggerMock = new Mock<ILogger<SolicitarPagoEfectivoCasoDeUso>>();
        }

        private SolicitarPagoEfectivoCasoDeUso CrearCasoDeUso() =>
            new SolicitarPagoEfectivoCasoDeUso(
                _comandaMockRepo.Object,
                _crearLlamadoServicioMock.Object,
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
                NumeroDeMesa = 5,
                MozoId = 1,
                Estado = EstadoComanda.EnEspera
            };

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

            _crearLlamadoServicioMock
                .Setup(s => s.CrearYNotificarAsync(comanda.MozoId, comanda.MesaId, comanda.NumeroDeMesa, CategoriaLlamado.Pago, It.IsAny<string>()))
                .ReturnsAsync(llamadoCreado);

            var resultado = await CrearCasoDeUso().EjecutarAsync(comandaId, restauranteId);
            Assert.NotNull(resultado);
            Assert.Equal((int)CategoriaLlamado.Pago, resultado.CategoriaLlamadoId);
            _crearLlamadoServicioMock.Verify(s => s.CrearYNotificarAsync(comanda.MozoId, comanda.MesaId, comanda.NumeroDeMesa, CategoriaLlamado.Pago, It.IsAny<string>()), Times.Once);
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