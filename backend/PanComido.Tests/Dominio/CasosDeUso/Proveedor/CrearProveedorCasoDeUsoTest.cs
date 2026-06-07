using Moq;
using PanComido.Dominio.CasosDeUso.PedidosCasosDeUso;
using PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Proveedor
{
    public class CrearProvedorCasoDeUsoTests
    {
        private readonly Mock<IProveedorRepositorio> _proveedorRepoMock;

        public CrearProvedorCasoDeUsoTests()
        {
            _proveedorRepoMock = new Mock<IProveedorRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTodosLosDatosSonValidos_SeCreaElNuevoProveedor()
        {
            int restauranteId = 1;
            string nombreProveedor = "Proveedor Test";
            string numeroTelefono = "123456789";
            List<int> categoriasId = new List<int>() {1, 2};

            var proveedorDominio = new DOM.Proveedor
            {
                Id = 0,
                RestauranteId = restauranteId,
                Nombre = nombreProveedor,
                NumeroTelefonoWsp = numeroTelefono,
                CategoriaIds = categoriasId
            };

            _proveedorRepoMock
                .Setup(r => r.ExisteProveedorConNombreAsync(restauranteId, It.IsAny<string>()))
                .ReturnsAsync(false);
            _proveedorRepoMock
                .Setup(r => r.CrearProveedorAsync(It.IsAny<DOM.Proveedor>()))
                .ReturnsAsync((DOM.Proveedor p) =>
                {
                    p.Id = 1;
                    return p;
                });

            var casoDeUso = new CrearProveedorCasoDeUso(_proveedorRepoMock.Object);

            var resultado = await casoDeUso.EjecutarAsync(proveedorDominio);

            Assert.NotNull(resultado);
            Assert.Equal(nombreProveedor, resultado.Nombre);
            Assert.Equal(1, resultado.Id);
            Assert.Equal(categoriasId, resultado.CategoriaIds);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElNombreEsVacio_LanzaArgumentException()
        {
            int restauranteId = 1;
            string nombreProveedor = "";
            string numeroTelefono = "123456789";
            List<int> categoriasId = new List<int>() { 1, 2 };

            var proveedorDominio = new DOM.Proveedor
            {
                Id = 0,
                RestauranteId = restauranteId,
                Nombre = nombreProveedor,
                NumeroTelefonoWsp = numeroTelefono,
                CategoriaIds = categoriasId
            };

            var casoDeUso = new CrearProveedorCasoDeUso(_proveedorRepoMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(proveedorDominio));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoTieneCategorias_LanzaArgumentException()
        {
            int restauranteId = 1;
            string nombreProveedor = "Proveedor Test";
            string numeroTelefono = "123456789";
            List<int> categoriasId = new List<int>() { };

            var proveedorDominio = new DOM.Proveedor
            {
                Id = 0,
                RestauranteId = restauranteId,
                Nombre = nombreProveedor,
                NumeroTelefonoWsp = numeroTelefono,
                CategoriaIds = categoriasId
            };

            var casoDeUso = new CrearProveedorCasoDeUso(_proveedorRepoMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(proveedorDominio));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoYaExisteUnProveedorConEseNombre_LanzaArgumentException()
        {
            int restauranteId = 1;
            string nombreProveedor = "Proveedor Test";
            string numeroTelefono = "123456789";
            List<int> categoriasId = new List<int>() {1,2};

            var proveedorDominio = new DOM.Proveedor
            {
                Id = 0,
                RestauranteId = restauranteId,
                Nombre = nombreProveedor,
                NumeroTelefonoWsp = numeroTelefono,
                CategoriaIds = categoriasId
            };

            var casoDeUso = new CrearProveedorCasoDeUso(_proveedorRepoMock.Object);

            _proveedorRepoMock
                .Setup(r => r.ExisteProveedorConNombreAsync(restauranteId, It.IsAny<string>()))
                .ReturnsAsync(true);
            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(proveedorDominio));
        }
    }
}