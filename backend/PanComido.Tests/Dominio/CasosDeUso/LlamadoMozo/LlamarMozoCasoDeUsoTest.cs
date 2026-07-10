using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.LlamadoMozoCasoDeUso;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.LlamadoMozo
{
    public class LlamarMozoCasoDeUsoTest
    {
        private readonly Mock<IMozoRepositorio> _mozoRepoMock;
        private readonly Mock<IMesaRepositorio> _mesaRepoMock;
        private readonly Mock<ICrearLlamadoServicio> _crearLlamadoServicioMock;
        private readonly Mock<ILogger<LlamarMozoCasoDeUso>> _loggerMock;

        public LlamarMozoCasoDeUsoTest()
        {
            _mozoRepoMock = new Mock<IMozoRepositorio>();
            _mesaRepoMock = new Mock<IMesaRepositorio>();
            _crearLlamadoServicioMock = new Mock<ICrearLlamadoServicio>();
            _loggerMock = new Mock<ILogger<LlamarMozoCasoDeUso>>();
        }

        private LlamarMozoCasoDeUso CrearCasoDeUso() =>
            new LlamarMozoCasoDeUso(
                _mozoRepoMock.Object,
                _mesaRepoMock.Object,
                _crearLlamadoServicioMock.Object,
                _loggerMock.Object);

        [Fact]
        public async Task EjecutarAsync_CuandoTodoEsValido_CreaElLlamado()
        {
            int restauranteId = 1;
            int mesaId = 1;
            string descripcion = "Necesito sal";

            var llamadoGuardado = new DOM.Llamado { Id = 1, MozoId = 1, MesaId = mesaId };

            _mozoRepoMock
                .Setup(r => r.ObtenerMozoAsignadoAMesaAsync(mesaId))
                .ReturnsAsync(1);

            _mesaRepoMock
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(new DOM.MesaConPosiciones { Id = mesaId });

            _crearLlamadoServicioMock
                .Setup(s => s.CrearYNotificarAsync(It.IsAny<int?>(), mesaId, It.IsAny<int>(), CategoriaLlamado.Sal, descripcion))
                .ReturnsAsync(llamadoGuardado);

            var resultado = await CrearCasoDeUso().EjecutarAsync(restauranteId, mesaId, CategoriaLlamado.Sal, descripcion);

            Assert.NotNull(resultado);
            _crearLlamadoServicioMock.Verify(s => s.CrearYNotificarAsync(It.IsAny<int?>(), mesaId, It.IsAny<int>(), CategoriaLlamado.Sal, descripcion), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoHayMozoAsignado_LanzaKeyNotFoundException()
        {
            int restauranteId = 1;
            int mesaId = 1;

            _mozoRepoMock
                .Setup(r => r.ObtenerMozoAsignadoAMesaAsync(mesaId))
                .ReturnsAsync(0);

            _mesaRepoMock
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(new DOM.MesaConPosiciones { Id = mesaId });

            await Assert.ThrowsAsync<KeyNotFoundException>(() => CrearCasoDeUso().EjecutarAsync(restauranteId, mesaId, CategoriaLlamado.General, null));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoLaMesaNoExiste_LanzaKeyNotFoundException()
        {
            int restauranteId = 1;
            int mesaId = 1;

            _mozoRepoMock
                .Setup(r => r.ObtenerMozoAsignadoAMesaAsync(mesaId))
                .ReturnsAsync(1);

            _mesaRepoMock
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync((DOM.MesaConPosiciones?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => CrearCasoDeUso().EjecutarAsync(restauranteId, mesaId, CategoriaLlamado.General, null));
        }
    }
}