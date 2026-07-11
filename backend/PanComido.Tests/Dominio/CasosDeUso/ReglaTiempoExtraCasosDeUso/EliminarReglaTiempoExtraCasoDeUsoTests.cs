using Moq;
using PanComido.Dominio.CasosDeUso.ReglaTiempoExtraCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.ReglaTiempoExtraCasosDeUso
{
    public class EliminarReglaTiempoExtraCasoDeUsoTests
    {
        private readonly Mock<IReglaTiempoExtraRepositorio> _mockRepositorio;
        private readonly EliminarReglaTiempoExtraCasoDeUso _casoDeUso;

        public EliminarReglaTiempoExtraCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IReglaTiempoExtraRepositorio>();
            _casoDeUso = new EliminarReglaTiempoExtraCasoDeUso(_mockRepositorio.Object);
        }

        [Fact]
        public async Task EjecutarAsync_ReglaNoExiste_LanzaKeyNotFoundException()
        {
            // Preparar
            int id = 1;
            int restauranteId = 1;

            _mockRepositorio.Setup(x => x.ObtenerPorIdAsync(id))
                .ReturnsAsync((ReglaTiempoExtra?)null);

            // Actuar & Verificar
            var excepcion = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _casoDeUso.EjecutarAsync(id, restauranteId));

            Assert.Equal("Regla no encontrada.", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_ReglaDeOtroRestaurante_LanzaKeyNotFoundException()
        {
            // Preparar
            int id = 1;
            int restauranteId = 1;
            var regla = new ReglaTiempoExtra { Id = id, RestauranteId = 99 }; // Diferente restaurante

            _mockRepositorio.Setup(x => x.ObtenerPorIdAsync(id))
                .ReturnsAsync(regla);

            // Actuar & Verificar
            var excepcion = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _casoDeUso.EjecutarAsync(id, restauranteId));

            Assert.Equal("Regla no encontrada.", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_CaminoFeliz_EliminaRegla()
        {
            // Preparar
            int id = 1;
            int restauranteId = 1;
            var regla = new ReglaTiempoExtra { Id = id, RestauranteId = restauranteId };

            _mockRepositorio.Setup(x => x.ObtenerPorIdAsync(id))
                .ReturnsAsync(regla);

            // Actuar
            await _casoDeUso.EjecutarAsync(id, restauranteId);

            // Verificar
            _mockRepositorio.Verify(x => x.EliminarAsync(id), Times.Once);
        }
    }
}
