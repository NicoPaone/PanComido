using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Proveedor
{
    public class ModificarProveedorCasoDeUsoTest
    {
        private readonly Mock<IProveedorRepositorio> _proveedorRepoMock;
        private readonly Mock<INormalizadorNombreServicio> _normalizadorNombreServicioMock;
        private readonly Mock<ILogger<ModificarProveedorCasoDeUso>> _loggerMock;

        public ModificarProveedorCasoDeUsoTest()
        {
            _proveedorRepoMock = new Mock<IProveedorRepositorio>();
            _normalizadorNombreServicioMock = new Mock<INormalizadorNombreServicio>();
            _normalizadorNombreServicioMock.Setup(s => s.Normalizar(It.IsAny<string>())).Returns((string nombre) => nombre);
            _loggerMock = new Mock<ILogger<ModificarProveedorCasoDeUso>>();
        }

        private ModificarProveedorCasoDeUso CrearCasoDeUso() =>
            new ModificarProveedorCasoDeUso(_proveedorRepoMock.Object, _normalizadorNombreServicioMock.Object, _loggerMock.Object);

        [Fact]
        public async Task EjecutarAsync_CuandoTodosLosDatosSonValidos_SeModificaElProveedor()
        {
            var proveedorDominio = new DOM.Proveedor
            {
                Id = 2,
                RestauranteId = 1,
                Nombre = "Proveedor Test",
                NumeroTelefonoWsp = "123456789",
                CategoriaIds = new List<int>() { 1, 2 }
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(2))
                .ReturnsAsync(new DOM.Proveedor { Id = 2, Nombre = "Proveedor de frutas" });

            _proveedorRepoMock
                .Setup(r => r.ExisteProveedorConNombreAsync(1, It.IsAny<string>()))
                .ReturnsAsync(false);

            _proveedorRepoMock
                .Setup(r => r.ModificarProveedorAsync(It.IsAny<DOM.Proveedor>()))
                .ReturnsAsync((DOM.Proveedor p) => { p.Id = 2; return p; });

            var resultado = await CrearCasoDeUso().EjecutarAsync(proveedorDominio);

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Id);
            _proveedorRepoMock.Verify(r => r.ModificarProveedorAsync(It.IsAny<DOM.Proveedor>()), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElProveedorNoExiste_LanzaKeyNotFoundException()
        {
            var proveedorDominio = new DOM.Proveedor
            {
                Id = 2,
                Nombre = "Proveedor Test",
                NumeroTelefonoWsp = "123456789",
                CategoriaIds = new List<int>() { 1, 2 }
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(2))
                .ReturnsAsync((DOM.Proveedor?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => CrearCasoDeUso().EjecutarAsync(proveedorDominio));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElNombreEsVacio_LanzaArgumentException()
        {
            var proveedorDominio = new DOM.Proveedor
            {
                Id = 2,
                Nombre = "",
                NumeroTelefonoWsp = "123456789",
                CategoriaIds = new List<int>() { 1, 2 }
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(2))
                .ReturnsAsync(new DOM.Proveedor { Id = 2, Nombre = "Proveedor de frutas" });

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(proveedorDominio));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoTieneCategorias_LanzaArgumentException()
        {
            var proveedorDominio = new DOM.Proveedor
            {
                Id = 2,
                Nombre = "Proveedor Test",
                NumeroTelefonoWsp = "123456789",
                CategoriaIds = new List<int>()
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(2))
                .ReturnsAsync(new DOM.Proveedor { Id = 2, Nombre = "Proveedor de frutas" });

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(proveedorDominio));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoYaExisteOtroProveedorConEseNombre_LanzaArgumentException()
        {
            var proveedorDominio = new DOM.Proveedor
            {
                Id = 2,
                RestauranteId = 1,
                Nombre = "Proveedor Test",
                NumeroTelefonoWsp = "123456789",
                CategoriaIds = new List<int>() { 1, 2 }
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(2))
                .ReturnsAsync(new DOM.Proveedor { Id = 2, Nombre = "Proveedor de frutas" });

            _proveedorRepoMock
                .Setup(r => r.ExisteProveedorConNombreAsync(1, It.IsAny<string>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(proveedorDominio));
        }
    }
}