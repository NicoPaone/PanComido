using Moq;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Tests.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class OcuparMesaCasoDeUsoTest
    {
        private readonly Mock<IMesaRepositorio> _mesaMockRepo;
        private readonly Mock<IComandaRepositorio> _comandaMockRepo;

        public OcuparMesaCasoDeUsoTest()
        {
            _mesaMockRepo = new Mock<IMesaRepositorio>();
            _comandaMockRepo = new Mock<IComandaRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTodoEsValido_OcupaLaMesaYDevuelveConComanda()
        {
            int restauranteId = 1;
            int mesaId = 1;
            int cantComensales = 2;
            int comandaNuevaId = 10;

            var mesaMock = new MesaConPosiciones
            {
                Id = mesaId,
                EstadoMesa = EstadoMesa.Disponible,
                CantPersonasMax = 4
            };

            _mesaMockRepo
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(mesaMock);

            _mesaMockRepo
                .Setup(r => r.ActualizarEstadoAsync(mesaId, EstadoMesa.Ocupada))
                .Returns(Task.CompletedTask);

            _comandaMockRepo
                .Setup(r => r.CrearAsync(It.IsAny<PanComido.Dominio.Entidades.Comanda>()))
                .ReturnsAsync(comandaNuevaId);

            var casoDeUso = new OcuparMesaCasoDeUso(_mesaMockRepo.Object, _comandaMockRepo.Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId, mesaId, cantComensales);

            Assert.NotNull(resultado);
            Assert.Equal(EstadoMesa.Ocupada, resultado.EstadoMesa);
            Assert.Equal(comandaNuevaId, resultado.idComanda);
            
            _comandaMockRepo.Verify(r => r.CrearAsync(It.IsAny<PanComido.Dominio.Entidades.Comanda>()), Times.Once());
            _mesaMockRepo.Verify(r => r.ActualizarEstadoAsync(mesaId, EstadoMesa.Ocupada), Times.Once());
        }

        [Fact]
        public async Task EjecutarAsync_CuandoMesaNoExiste_LanzaArgumentException()
        {
            int restauranteId = 1;
            int mesaId = 1;

            _mesaMockRepo
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync((MesaConPosiciones)null);

            var casoDeUso = new OcuparMesaCasoDeUso(_mesaMockRepo.Object, _comandaMockRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(restauranteId, mesaId, 2));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoMesaNoEstaDisponible_LanzaInvalidOperationException()
        {
            int restauranteId = 1;
            int mesaId = 1;

            _mesaMockRepo
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(new MesaConPosiciones { EstadoMesa = EstadoMesa.Ocupada });

            var casoDeUso = new OcuparMesaCasoDeUso(_mesaMockRepo.Object, _comandaMockRepo.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => casoDeUso.EjecutarAsync(restauranteId, mesaId, 2));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoExcedeCapacidad_LanzaInvalidOperationException()
        {
            int restauranteId = 1;
            int mesaId = 1;

            _mesaMockRepo
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(new MesaConPosiciones { EstadoMesa = EstadoMesa.Disponible, CantPersonasMax = 2 });

            var casoDeUso = new OcuparMesaCasoDeUso(_mesaMockRepo.Object, _comandaMockRepo.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => casoDeUso.EjecutarAsync(restauranteId, mesaId, 5));
        }
    }
}
