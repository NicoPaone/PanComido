using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.LlamadoMozoCasoDeUso;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Tests.Dominio.CasosDeUso.LlamadoMozo
{
    public class ResolverLlamadoCasoDeUsoTest
    {
        private readonly Mock<ILlamadoRepositorio> _llamadoRepoMock;
        private readonly Mock<ILogger<ResolverLlamadoCasoDeUso>> _loggerMock;

        public ResolverLlamadoCasoDeUsoTest()
        {
            _llamadoRepoMock = new Mock<ILlamadoRepositorio>();
            _loggerMock = new Mock<ILogger<ResolverLlamadoCasoDeUso>>();
        }

        private ResolverLlamadoCasoDeUso CrearCasoDeUso() =>
            new ResolverLlamadoCasoDeUso(_llamadoRepoMock.Object, _loggerMock.Object);

        [Fact]
        public async Task EjecutarAsync_CuandoElLlamadoExiste_ResuelveLlamado()
        {
            int llamadoId = 1;

            _llamadoRepoMock
                .Setup(r => r.ResolverLlamadoAsync(llamadoId))
                .ReturnsAsync(true);

            await CrearCasoDeUso().EjecutarAsync(llamadoId);

            _llamadoRepoMock.Verify(r => r.ResolverLlamadoAsync(llamadoId), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElLlamadoNoExiste_LanzaKeyNotFoundException()
        {
            int llamadoId = 1;

            _llamadoRepoMock
                .Setup(r => r.ResolverLlamadoAsync(llamadoId))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => CrearCasoDeUso().EjecutarAsync(llamadoId));
        }
    }
}