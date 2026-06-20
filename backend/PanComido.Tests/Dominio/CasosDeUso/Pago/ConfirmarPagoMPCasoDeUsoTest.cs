using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.PagoCasoDeUso;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Dominio.Interfaces.Servicios.MercadoPago;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Pago
{
    public class ConfirmarPagoMPCasoDeUsoTest
    {
        private readonly Mock<IMercadoPagoServicio> _mercadoPagoMockServicio;
        private readonly Mock<IPagoRepositorio> _pagoMockRepo;
        private readonly Mock<IComandaRepositorio> _comandaMockRepo;
        private readonly Mock<IComandaNotificador> _comandaNotificadorMock;
        private readonly Mock<ILogger<ConfirmarPagoMPCasoDeUso>> _loggerMock;

        public ConfirmarPagoMPCasoDeUsoTest()
        {
            _mercadoPagoMockServicio = new Mock<IMercadoPagoServicio>();
            _pagoMockRepo = new Mock<IPagoRepositorio>();
            _comandaMockRepo = new Mock<IComandaRepositorio>();
            _comandaNotificadorMock = new Mock<IComandaNotificador>();
            _loggerMock = new Mock<ILogger<ConfirmarPagoMPCasoDeUso>>();
        }

        private ConfirmarPagoMPCasoDeUso CrearCasoDeUso() =>
            new ConfirmarPagoMPCasoDeUso(
                _mercadoPagoMockServicio.Object,
                _pagoMockRepo.Object,
                _comandaMockRepo.Object,
                _comandaNotificadorMock.Object,
                _loggerMock.Object);

        [Fact]
        public async Task EjecutarAsync_CuandoElPagoEsAprobado_ConfirmaYFinalizaLaComanda()
        {
            long paymentId = 123;
            string externalReference = "Comanda-1";

            var resultadoMP = new DOM.ResultadoPagoMP
            {
                ExternalReference = externalReference,
                Status = "approved"
            };

            var pago = new DOM.Pago
            {
                PagoId = 1,
                ComandaId = 1,
                ExternalReference = externalReference,
                EstadoPago = EstadoPago.Pendiente
            };

            var comanda = new DOM.Comanda { Id = 1, Estado = EstadoComanda.EnEspera };

            _mercadoPagoMockServicio
                .Setup(s => s.ConsultarPagoAsync(paymentId))
                .ReturnsAsync(resultadoMP);

            _pagoMockRepo
                .Setup(r => r.ObtenerPagoPorExternalReferenceAsync(externalReference))
                .ReturnsAsync(pago);

            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(pago.ComandaId))
                .ReturnsAsync(comanda);

            _pagoMockRepo
                .Setup(r => r.ConfirmarPagoAsync(externalReference))
                .ReturnsAsync(new DOM.Pago());

            _comandaMockRepo
                .Setup(r => r.ActualizarAsync(It.IsAny<DOM.Comanda>()))
                .Returns(Task.CompletedTask);

            _comandaNotificadorMock
                .Setup(n => n.NotificarComandaActualizadaAMesaAsync(It.IsAny<DOM.Comanda>()))
                .Returns(Task.CompletedTask);

            var resultado = await CrearCasoDeUso().EjecutarAsync(paymentId);

            Assert.NotNull(resultado);
            Assert.Equal(EstadoComanda.Finalizada, comanda.Estado);
            _pagoMockRepo.Verify(r => r.ConfirmarPagoAsync(externalReference), Times.Once);
            _comandaMockRepo.Verify(r => r.ActualizarAsync(It.IsAny<DOM.Comanda>()), Times.Once);
            _comandaNotificadorMock.Verify(n => n.NotificarComandaActualizadaAMesaAsync(It.IsAny<DOM.Comanda>()), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElPagoEsRechazado_RechazaYNotificaALaMesa()
        {
            long paymentId = 123;
            string externalReference = "Comanda-1";

            var resultadoMP = new DOM.ResultadoPagoMP
            {
                ExternalReference = externalReference,
                Status = "rejected"
            };

            var pago = new DOM.Pago
            {
                PagoId = 1,
                ComandaId = 1,
                ExternalReference = externalReference,
                EstadoPago = EstadoPago.Pendiente
            };

            var comanda = new DOM.Comanda { Id = 1 };

            _mercadoPagoMockServicio
                .Setup(s => s.ConsultarPagoAsync(paymentId))
                .ReturnsAsync(resultadoMP);

            _pagoMockRepo
                .Setup(r => r.ObtenerPagoPorExternalReferenceAsync(externalReference))
                .ReturnsAsync(pago);

            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(pago.ComandaId))
                .ReturnsAsync(comanda);

            _pagoMockRepo
                .Setup(r => r.RechazarPagoAsync(externalReference))
                .ReturnsAsync(new DOM.Pago());

            _comandaNotificadorMock
                .Setup(n => n.NotificarPagoRechazadoAMesaAsync(It.IsAny<DOM.Comanda>()))
                .Returns(Task.CompletedTask);

            var resultado = await CrearCasoDeUso().EjecutarAsync(paymentId);

            Assert.Null(resultado);
            _pagoMockRepo.Verify(r => r.RechazarPagoAsync(externalReference), Times.Once);
            _comandaNotificadorMock.Verify(n => n.NotificarPagoRechazadoAMesaAsync(It.IsAny<DOM.Comanda>()), Times.Once);
            _pagoMockRepo.Verify(r => r.ConfirmarPagoAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElPagoNoExiste_LanzaKeyNotFoundException()
        {
            long paymentId = 123;
            string externalReference = "Comanda-1";

            _mercadoPagoMockServicio
                .Setup(s => s.ConsultarPagoAsync(paymentId))
                .ReturnsAsync(new DOM.ResultadoPagoMP { ExternalReference = externalReference });

            _pagoMockRepo
                .Setup(r => r.ObtenerPagoPorExternalReferenceAsync(externalReference))
                .ReturnsAsync((DOM.Pago?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => CrearCasoDeUso().EjecutarAsync(paymentId));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElPagoYaEstaConfirmado_RetornaNullSinProcesar()
        {
            long paymentId = 123;
            string externalReference = "Comanda-1";

            var resultadoMP = new DOM.ResultadoPagoMP
            {
                ExternalReference = externalReference,
                Status = "approved"
            };

            var pago = new DOM.Pago
            {
                PagoId = 1,
                ComandaId = 1,
                ExternalReference = externalReference,
                EstadoPago = EstadoPago.Confirmado
            };

            _mercadoPagoMockServicio
                .Setup(s => s.ConsultarPagoAsync(paymentId))
                .ReturnsAsync(resultadoMP);

            _pagoMockRepo
                .Setup(r => r.ObtenerPagoPorExternalReferenceAsync(externalReference))
                .ReturnsAsync(pago);

            _comandaMockRepo
                .Setup(r => r.ObtenerComandaPorIdAsync(pago.ComandaId))
                .ReturnsAsync(new DOM.Comanda { Id = 1 });

            var resultado = await CrearCasoDeUso().EjecutarAsync(paymentId);

            Assert.Null(resultado);
            _pagoMockRepo.Verify(r => r.ConfirmarPagoAsync(It.IsAny<string>()), Times.Never);
            _comandaMockRepo.Verify(r => r.ActualizarAsync(It.IsAny<DOM.Comanda>()), Times.Never);
        }
    }
}