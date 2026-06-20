using Moq;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Tests.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class GuardarMapaCasoDeUsoTest
    {
        private readonly Mock<IMesaRepositorio> _mesaMockRepo;

        public GuardarMapaCasoDeUsoTest()
        {
            _mesaMockRepo = new Mock<IMesaRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoMesasNoSeSuperponen_GuardaElMapa()
        {
            int restauranteId = 1;
            var mesas = new List<MesaMapaDominio>
            {
                new MesaMapaDominio { Numero = 1, PosicionXInicio = 0, PosicionXFin = 10, PosicionYInicio = 0, PosicionYFin = 10 },
                new MesaMapaDominio { Numero = 2, PosicionXInicio = 20, PosicionXFin = 30, PosicionYInicio = 20, PosicionYFin = 30 }
            };

            _mesaMockRepo
                .Setup(r => r.GuardarMapaMasivoAsync(restauranteId, mesas))
                .Returns(Task.CompletedTask);

            var casoDeUso = new GuardarMapaCasoDeUso(_mesaMockRepo.Object);

            await casoDeUso.EjecutarAsync(restauranteId, mesas);

            _mesaMockRepo.Verify(r => r.GuardarMapaMasivoAsync(restauranteId, mesas), Times.Once());
        }

        [Fact]
        public async Task EjecutarAsync_CuandoMesasSeSuperponen_LanzaInvalidOperationException()
        {
            int restauranteId = 1;
            var mesas = new List<MesaMapaDominio>
            {
                new MesaMapaDominio { Numero = 1, PosicionXInicio = 0, PosicionXFin = 10, PosicionYInicio = 0, PosicionYFin = 10 },
                new MesaMapaDominio { Numero = 2, PosicionXInicio = 5, PosicionXFin = 15, PosicionYInicio = 5, PosicionYFin = 15 }
            };

            var casoDeUso = new GuardarMapaCasoDeUso(_mesaMockRepo.Object);

            var excepcion = await Assert.ThrowsAsync<InvalidOperationException>(() => casoDeUso.EjecutarAsync(restauranteId, mesas));
            Assert.Contains("están superpuestas", excepcion.Message);
            
            _mesaMockRepo.Verify(r => r.GuardarMapaMasivoAsync(It.IsAny<int>(), It.IsAny<List<MesaMapaDominio>>()), Times.Never());
        }
    }
}
