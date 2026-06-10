using Moq;
using PanComido.Dominio.CasosDeUso.LlamadoMozoCasoDeUso;
using PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.LlamadoMozo
{
    public class ListarLlamadosPendientesCasoDeUsoTest
    {
        private readonly Mock<ILlamadoRepositorio> _llamadoMockRepo;

        public ListarLlamadosPendientesCasoDeUsoTest() 
        {
            _llamadoMockRepo = new Mock<ILlamadoRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHayLlamadosPendientes_DevuelveLista()
        {
            int mozoId = 1;

            var llamados = new List<DOM.Llamado>
            {
                new DOM.Llamado { Id = 1, MozoId = mozoId },
                new DOM.Llamado { Id = 2, MozoId = mozoId }
            };

            _llamadoMockRepo
                .Setup(r => r.ObtenerLlamadosPendientesPorMozoAsync(mozoId))
                .ReturnsAsync(llamados);

            var casoDeUso = new ListarLlamadosPendientesCasoDeUso(_llamadoMockRepo.Object);

            var resultado = await casoDeUso.EjecutarAsync(mozoId);
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }

    }
}
