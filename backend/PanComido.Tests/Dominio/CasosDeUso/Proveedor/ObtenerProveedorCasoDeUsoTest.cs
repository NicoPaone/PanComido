using Moq;
using PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;


namespace PanComido.Tests.Dominio.CasosDeUso.Proveedor
{
    public class ObtenerProveedorCasoDeUsoTest
    {
        private readonly Mock<IProveedorRepositorio> _proveedorMockRepo;

        public ObtenerProveedorCasoDeUsoTest()
        {
            _proveedorMockRepo = new Mock<IProveedorRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElProveedorExiste_DevuelveElProveedor()
        {
            int proveedorId = 1;

            var proveedor = new DOM.Proveedor
            {
                Id = proveedorId,
                Nombre = "Proveedor Test",
                NumeroTelefonoWsp = "1151259442"
            };

            _proveedorMockRepo
                .Setup(r => r.ObtenerProveedorPorIdAsync(proveedorId))
                .ReturnsAsync(proveedor);

            var casoDeUso = new ObtenerProveedorCasoDeUso(_proveedorMockRepo.Object);

            var resultado = await casoDeUso.EjecutarAsync(proveedorId);
            Assert.NotNull(resultado);
            Assert.Equal(proveedorId, resultado.Id);
            Assert.Equal("Proveedor Test", resultado.Nombre);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElProveedorNoExiste_LanzaKeyNotFoundException()
        {
            int proveedorId = 1;

            _proveedorMockRepo
                .Setup(r => r.ObtenerProveedorPorIdAsync(proveedorId))
                .ReturnsAsync((DOM.Proveedor?)null);

            var casoDeUso = new ObtenerProveedorCasoDeUso(_proveedorMockRepo.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(proveedorId));
        }
    }
}
