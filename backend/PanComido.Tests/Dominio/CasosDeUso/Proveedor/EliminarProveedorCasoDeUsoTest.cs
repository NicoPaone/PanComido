using Moq;
using PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Proveedor
{
    public class EliminarProveedorCasoDeUsoTest
    {
        private readonly Mock<IProveedorRepositorio> _proveedorRepoMock;

        public EliminarProveedorCasoDeUsoTest()
        {
            _proveedorRepoMock = new Mock<IProveedorRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElProveedorExiste_SeEliminaElProveedor()
        {
            int restauranteId = 1;
            string nombreProveedor = "Proveedor Test";
            string numeroTelefono = "1154896312";
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
                    Nombre = "Proveedor Test",
                    NumeroTelefonoWsp = "1154896312",
                    CategoriaIds = new List<int>() { 1, 2 }
                });


            _proveedorRepoMock
                .Setup(r => r.EliminarProveedorAsync(proveedorDominio.Id))
                .Returns(Task.CompletedTask);

            var casoDeUso = new EliminarProveedorCasoDeuso(_proveedorRepoMock.Object);

            await casoDeUso.EjecutarAsync(proveedorDominio.Id);
            _proveedorRepoMock.Verify(r => r.EliminarProveedorAsync(proveedorDominio.Id), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElProveedorNoExiste_LanzaKeyNotFoundException()
        {
            int restauranteId = 1;
            string nombreProveedor = "Proveedor Test";
            string numeroTelefono = "1154896312";
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
                .ReturnsAsync((DOM.Proveedor?)null);

            var casoDeUso = new EliminarProveedorCasoDeuso(_proveedorRepoMock.Object);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(proveedorDominio.Id));
        }
    }
}
