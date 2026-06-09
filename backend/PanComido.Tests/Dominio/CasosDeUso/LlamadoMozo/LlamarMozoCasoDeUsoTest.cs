using Moq;
using PanComido.Dominio.CasosDeUso.LlamadoMozoCasoDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.LlamadoMozo
{
    public class LlamarMozoCasoDeUsoTest
    {
        private readonly Mock<IMozoRepositorio> _mozoRepoMock;
        private readonly Mock<ILlamadoRepositorio> _llamadoRepoMock;
        private readonly Mock<ILlamadoNotificador> _llamadoNotificadorRepoMock;
        private readonly Mock<IMesaRepositorio> _mesaRepoMock;

        public LlamarMozoCasoDeUsoTest()
        {
            _mozoRepoMock = new Mock<IMozoRepositorio>();
            _llamadoRepoMock = new Mock<ILlamadoRepositorio>();
            _llamadoNotificadorRepoMock = new Mock<ILlamadoNotificador>();
            _mesaRepoMock = new Mock<IMesaRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTodoEsValido_CreaElLlamado()
        {
            int restauranteId = 1;
            int mesaId = 1;
            int categoriaLlamadoId = 2;
            string descripcion = "Necesito sal";

            var mesa = new DOM.Mesa { Id = mesaId };
            var llamadoGuardado = new DOM.Llamado { Id = 1, MozoId = 1, MesaId = mesaId };

            _mozoRepoMock
                .Setup(r => r.ObtenerMozoAsignadoAMesaAsync(mesaId))
                .ReturnsAsync(1);

            _mesaRepoMock
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(new DOM.MesaConPosiciones { Id = mesaId });

            _llamadoRepoMock
                .Setup(r => r.crearLlamadoAsync(It.IsAny<DOM.Llamado>()))
                .ReturnsAsync(llamadoGuardado);

            _llamadoNotificadorRepoMock
                .Setup(r => r.NotificarLlamadoAsync(It.IsAny<DOM.Llamado>()))
                .Returns(Task.CompletedTask);

            var casoDeUso = new LlamarMozoCasoDeUso(
                 _mozoRepoMock.Object,
                 _llamadoRepoMock.Object,
                 _llamadoNotificadorRepoMock.Object,
                 _mesaRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(restauranteId, mesaId, categoriaLlamadoId, descripcion);
            Assert.NotNull(resultado);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoHayMozoAsignado_LanzaKeyNotFoundException()
        {
            int restauranteId = 1;
            int mesaId = 1;
            int categoriaLlamadoId = 1;
            string descripcion = "Necesito agua";

            var mesa = new DOM.Mesa { Id = mesaId };
            var llamadoGuardado = new DOM.Llamado { Id = 1, MozoId = 1, MesaId = mesaId };

            _mozoRepoMock
                .Setup(r => r.ObtenerMozoAsignadoAMesaAsync(mesaId))
                .ReturnsAsync(0);

            _mesaRepoMock
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(new DOM.MesaConPosiciones { Id = mesaId });

            var casoDeUso = new LlamarMozoCasoDeUso(
                 _mozoRepoMock.Object,
                 _llamadoRepoMock.Object,
                 _llamadoNotificadorRepoMock.Object,
                 _mesaRepoMock.Object);


            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(restauranteId, mesaId, categoriaLlamadoId, descripcion));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaMesaNoExiste_LanzaKeyNotFoundException()
        {
            int restauranteId = 1;
            int mesaId = 1;
            int categoriaLlamadoId = 4;
            string descripcion = "";

            var mesa = new DOM.Mesa { Id = mesaId };
            var llamadoGuardado = new DOM.Llamado { Id = 1, MozoId = 1, MesaId = mesaId };

            _mozoRepoMock
                .Setup(r => r.ObtenerMozoAsignadoAMesaAsync(mesaId))
                .ReturnsAsync(1);

            _mesaRepoMock
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync((DOM.MesaConPosiciones?)null);

            var casoDeUso = new LlamarMozoCasoDeUso(
                 _mozoRepoMock.Object,
                 _llamadoRepoMock.Object,
                 _llamadoNotificadorRepoMock.Object,
                 _mesaRepoMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(restauranteId, mesaId, categoriaLlamadoId, descripcion));

        }
    }
}
