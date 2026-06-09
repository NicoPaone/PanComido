using Moq;
using PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Entidades;
using DOM = PanComido.Dominio.Entidades;


namespace PanComido.Tests.Dominio.CasosDeUso.Proveedor
{
    public class ModificarProveedorCasoDeUsoTest
    {
        private readonly Mock<IProveedorRepositorio> _proveedorRepoMock;

        public ModificarProveedorCasoDeUsoTest()
        {
            _proveedorRepoMock = new Mock<IProveedorRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTodosLosDatosSonValidos_SeModificaElProveedor()
        {
            int restauranteId = 1;

            string nombreProveedor = "Proveedor Test";
            string numeroTelefono = "123456789";
            List<int> categoriasId = new List<int>() { 1, 2 };

            var proveedorDominio = new DOM.Proveedor
            {
                Id = 2,
                Nombre = nombreProveedor,
                NumeroTelefonoWsp = numeroTelefono,
                CategoriaIds = categoriasId
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(proveedorDominio.Id))
                .ReturnsAsync(new DOM.Proveedor
                {
                    Id = 2,
                    Nombre = "Proveedor de frutas",
                    NumeroTelefonoWsp = "1154896312",
                    CategoriaIds = new List<int>() { 1, 2 }
                });

            _proveedorRepoMock
                .Setup(r => r.ExisteProveedorConNombreAsync(restauranteId, It.IsAny<string>()))
                .ReturnsAsync(false);

            _proveedorRepoMock
                .Setup(r => r.ModificarProveedorAsync(It.IsAny<DOM.Proveedor>()))
                 .ReturnsAsync((DOM.Proveedor p) =>
                 {
                     p.Id = 2;
                     return p;
                 });

            var casoDeUso = new ModificarProveedorCasoDeUso(_proveedorRepoMock.Object);
            var resultado = await casoDeUso.EjecutarAsync(proveedorDominio);

            Assert.NotNull(resultado);
            Assert.Equal(nombreProveedor, resultado.Nombre);
            Assert.Equal(2, resultado.Id);
            Assert.Equal(categoriasId, resultado.CategoriaIds);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElProveedorNoExiste_LanzaKeyNotFoundException()
        {
            int restauranteId = 1;

            string nombreProveedor = "Proveedor Test";
            string numeroTelefono = "123456789";
            List<int> categoriasId = new List<int>() { 1, 2 };

            var proveedorDominio = new DOM.Proveedor
            {
                Id = 2,
                Nombre = nombreProveedor,
                NumeroTelefonoWsp = numeroTelefono,
                CategoriaIds = categoriasId
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(proveedorDominio.Id))
                .ReturnsAsync((DOM.Proveedor?)null);

            var casoDeUso = new ModificarProveedorCasoDeUso(_proveedorRepoMock.Object);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(proveedorDominio));
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
                Id = 2,
                Nombre = nombreProveedor,
                NumeroTelefonoWsp = numeroTelefono,
                CategoriaIds = categoriasId
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(proveedorDominio.Id))
                .ReturnsAsync(new DOM.Proveedor
                {
                    Id = 2,
                    Nombre = "Proveedor de frutas",
                    NumeroTelefonoWsp = "1154896312",
                    CategoriaIds = new List<int>() { 1, 2 }
                });

            var casoDeUso = new ModificarProveedorCasoDeUso(_proveedorRepoMock.Object);
            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(proveedorDominio));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoTieneCategorias_LanzaArgumentException()
        {
            int restauranteId = 1;

            string nombreProveedor = "";
            string numeroTelefono = "123456789";
            List<int> categoriasId = new List<int>() { };

            var proveedorDominio = new DOM.Proveedor
            {
                Id = 2,
                Nombre = nombreProveedor,
                NumeroTelefonoWsp = numeroTelefono,
                CategoriaIds = categoriasId
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(proveedorDominio.Id))
                .ReturnsAsync(new DOM.Proveedor
                {
                    Id = 2,
                    Nombre = "Proveedor de frutas",
                    NumeroTelefonoWsp = "1154896312",
                    CategoriaIds = new List<int>() { 1, 2 }
                });

            var casoDeUso = new ModificarProveedorCasoDeUso(_proveedorRepoMock.Object);
            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(proveedorDominio));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoYaExisteOtroProveedorConEseNombre_LanzaArgumentException()
        {
            int restauranteId = 1;

            string nombreProveedor = "Proveedor Test";
            string numeroTelefono = "123456789";
            List<int> categoriasId = new List<int>() { 1, 2 };

            var proveedorDominio = new DOM.Proveedor
            {
                Id = 2,
                RestauranteId = restauranteId,
                Nombre = nombreProveedor,
                NumeroTelefonoWsp = numeroTelefono,
                CategoriaIds = categoriasId
            };

            _proveedorRepoMock
                .Setup(r => r.ObtenerProveedorPorIdAsync(proveedorDominio.Id))
                .ReturnsAsync(new DOM.Proveedor
                {
                    Id = 2,
                    Nombre = "Proveedor de frutas",
                    NumeroTelefonoWsp = "1154896312",
                    CategoriaIds = new List<int>() { 1, 2 }
                });

            _proveedorRepoMock
                .Setup(r => r.ExisteProveedorConNombreAsync(restauranteId, It.IsAny<string>()))
                .ReturnsAsync(true);

            var casoDeUso = new ModificarProveedorCasoDeUso(_proveedorRepoMock.Object);
            await Assert.ThrowsAsync<ArgumentException>(() => casoDeUso.EjecutarAsync(proveedorDominio));
        }
    }
}
