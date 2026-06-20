using Moq;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Tests.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class CambiarEstadoMesaCasoDeUsoTest
    {
        private readonly Mock<IMesaRepositorio> _mesaMockRepo;

        public CambiarEstadoMesaCasoDeUsoTest()
        {
            _mesaMockRepo = new Mock<IMesaRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaMesaExiste_CambiaElEstadoYDevuelveLaMesa()
        {
            int restauranteId = 1;
            int mesaId = 1;
            var estadoNuevo = EstadoMesa.Ocupada;

            var mesaDevuelta = new MesaConPosiciones
            {
                Id = mesaId,
                EstadoMesa = EstadoMesa.Disponible
            };

            _mesaMockRepo
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(mesaDevuelta);

            _mesaMockRepo
                .Setup(r => r.ActualizarEstadoAsync(mesaId, estadoNuevo))
                .Returns(Task.CompletedTask);

            var casoDeUso = new CambiarEstadoMesaCasoDeUso(_mesaMockRepo.Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId, mesaId, estadoNuevo);

            Assert.NotNull(resultado);
            Assert.Equal(estadoNuevo, resultado.EstadoMesa);
            _mesaMockRepo.Verify(r => r.ObtenerPorIdAsync(mesaId, restauranteId), Times.Once());
            _mesaMockRepo.Verify(r => r.ActualizarEstadoAsync(mesaId, estadoNuevo), Times.Once());
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaMesaNoExiste_LanzaArgumentException()
        {
            int restauranteId = 1;
            int mesaId = 99;

            _mesaMockRepo
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync((MesaConPosiciones)null);

            var casoDeUso = new CambiarEstadoMesaCasoDeUso(_mesaMockRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(restauranteId, mesaId, EstadoMesa.Ocupada));
            
            _mesaMockRepo.Verify(r => r.ObtenerPorIdAsync(mesaId, restauranteId), Times.Once());
            _mesaMockRepo.Verify(r => r.ActualizarEstadoAsync(It.IsAny<int>(), It.IsAny<EstadoMesa>()), Times.Never());
        }
    }
}
