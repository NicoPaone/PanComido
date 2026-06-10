using Moq;
using PanComido.Dominio.CasosDeUso.LlamadoMozoCasoDeUso;
using PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Tests.Dominio.CasosDeUso.LlamadoMozo
{
    public class ResolverLlamadoCasoDeUsoTest
    {
        private readonly Mock<ILlamadoRepositorio> _llamadoRepoMock;

        public ResolverLlamadoCasoDeUsoTest()
        {
            _llamadoRepoMock = new Mock<ILlamadoRepositorio>();
        }


        [Fact]
        public async Task EjecutarAsync_CuandoElLlamadoExiste_ResuelveLlamado()
        {
            int llamadoId = 1;

            _llamadoRepoMock
                .Setup(r => r.ResolverLlamadoAsync(llamadoId))
                .ReturnsAsync(true);

            var casoDeUso = new ResolverLlamadoCasoDeUso(_llamadoRepoMock.Object);

            await casoDeUso.EjecutarAsync(llamadoId);

            _llamadoRepoMock.Verify(r => r.ResolverLlamadoAsync(llamadoId), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElLlamadoNoExiste_LanzaKeyNotFoundException()
        {
            int llamadoId = 1;

            _llamadoRepoMock
                .Setup(r => r.ResolverLlamadoAsync(llamadoId))
                .ReturnsAsync(false);

            var casoDeUso = new ResolverLlamadoCasoDeUso(_llamadoRepoMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(llamadoId));
        }
    }
}
