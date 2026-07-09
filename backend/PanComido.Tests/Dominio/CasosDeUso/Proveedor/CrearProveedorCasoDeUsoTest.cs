using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Proveedor
{
    public class CrearProvedorCasoDeUsoTests
    {
        private readonly Mock<IProveedorRepositorio> _proveedorRepoMock;
        private readonly Mock<INormalizadorNombreServicio> _normalizadorNombreServicioMock;
        private readonly Mock<ILogger<CrearProveedorCasoDeUso>> _loggerMock;

        public CrearProvedorCasoDeUsoTests()
        {
            _proveedorRepoMock = new Mock<IProveedorRepositorio>();
            _normalizadorNombreServicioMock = new Mock<INormalizadorNombreServicio>();
            _normalizadorNombreServicioMock.Setup(s => s.Normalizar(It.IsAny<string>())).Returns((string nombre) => nombre);
            _loggerMock = new Mock<ILogger<CrearProveedorCasoDeUso>>();
        }

        private CrearProveedorCasoDeUso CrearCasoDeUso() =>
            new CrearProveedorCasoDeUso(_proveedorRepoMock.Object, _normalizadorNombreServicioMock.Object, _loggerMock.Object);

        [Fact]
        public async Task EjecutarAsync_CuandoTodosLosDatosSonValidos_SeCreaElNuevoProveedor()
        {
            int restauranteId = 1;
            var proveedorDominio = new DOM.Proveedor
            {
                Id = 0,
                RestauranteId = restauranteId,
                Nombre = "Proveedor Test",
                NumeroTelefonoWsp = "123456789",
                CategoriaIds = new List<int>() { 1, 2 }
            };

            _proveedorRepoMock
                .Setup(r => r.ExisteProveedorConNombreAsync(restauranteId, It.IsAny<string>()))
                .ReturnsAsync(false);
            _proveedorRepoMock
                .Setup(r => r.CrearProveedorAsync(It.IsAny<DOM.Proveedor>()))
                .ReturnsAsync((DOM.Proveedor p) => { p.Id = 1; return p; });

            var resultado = await CrearCasoDeUso().EjecutarAsync(proveedorDominio);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            _proveedorRepoMock.Verify(r => r.CrearProveedorAsync(It.IsAny<DOM.Proveedor>()), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElNombreEsVacio_LanzaArgumentException()
        {
            var proveedorDominio = new DOM.Proveedor
            {
                Id = 0,
                RestauranteId = 1,
                Nombre = "",
                NumeroTelefonoWsp = "123456789",
                CategoriaIds = new List<int>() { 1, 2 }
            };

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(proveedorDominio));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoTieneCategorias_LanzaArgumentException()
        {
            var proveedorDominio = new DOM.Proveedor
            {
                Id = 0,
                RestauranteId = 1,
                Nombre = "Proveedor Test",
                NumeroTelefonoWsp = "123456789",
                CategoriaIds = new List<int>()
            };

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(proveedorDominio));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElNumeroTelefonoWspEsVacio_LanzaArgumentException()
        {
            var proveedorDominio = new DOM.Proveedor
            {
                Id = 0,
                RestauranteId = 1,
                Nombre = "Proveedor Test",
                NumeroTelefonoWsp = "",
                CategoriaIds = new List<int>() { 1, 2 }
            };

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(proveedorDominio));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoYaExisteUnProveedorConEseNombre_LanzaArgumentException()
        {
            var proveedorDominio = new DOM.Proveedor
            {
                Id = 0,
                RestauranteId = 1,
                Nombre = "Proveedor Test",
                NumeroTelefonoWsp = "123456789",
                CategoriaIds = new List<int>() { 1, 2 }
            };

            _proveedorRepoMock
                .Setup(r => r.ExisteProveedorConNombreAsync(1, It.IsAny<string>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(proveedorDominio));
        }
    }
}