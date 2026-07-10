using Moq;
using PanComido.Dominio.CasosDeUso.MiseAndPlaceCasoDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.MiseAndPlace
{
    public class ProducirMiseAndPlaceCasoDeUsoTest
    {
        private readonly Mock<IMiseAndPlaceRepositorio> _miseAndPlaceRepoMock;
        private readonly Mock<IGestionStockServicio> _gestionStockServicioMock;
        private readonly Mock<IGeneradorNombreLoteServicio> _generadorNombreLoteServicioMock;
        private readonly ProducirMiseAndPlaceCasoDeUso _casoDeUso;

        public ProducirMiseAndPlaceCasoDeUsoTest()
        {
            _miseAndPlaceRepoMock = new Mock<IMiseAndPlaceRepositorio>();
            _gestionStockServicioMock = new Mock<IGestionStockServicio>();
            _generadorNombreLoteServicioMock = new Mock<IGeneradorNombreLoteServicio>();
            _casoDeUso = new ProducirMiseAndPlaceCasoDeUso(
                _miseAndPlaceRepoMock.Object,
                _gestionStockServicioMock.Object,
                _generadorNombreLoteServicioMock.Object);
        }

        private static MiseAndPlaceListadoDominio CrearMiseAndPlaceExistente()
        {
            return new MiseAndPlaceListadoDominio
            {
                MiseAndPlaceId = 10,
                Nombre = "Salsa Base",
                Receta = new List<RecetaItemDominio>
                {
                    new RecetaItemDominio { IngredienteId = 1, Cantidad = 2 },
                    new RecetaItemDominio { IngredienteId = 2, Cantidad = 3 }
                }
            };
        }

        [Fact]
        public async Task EjecutarAsync_CantidadMenorOIgualACero_LanzaArgumentException()
        {
            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(1, 10, 0, DateOnly.FromDateTime(DateTime.Today), 1));

            Assert.Equal("La cantidad a producir debe ser mayor a cero.", excepcion.Message);
            _miseAndPlaceRepoMock.Verify(r => r.ObtenerPorIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_MiseAndPlaceNoExiste_LanzaArgumentException()
        {
            // Preparar
            _miseAndPlaceRepoMock
                .Setup(r => r.ObtenerPorIdAsync(1, 10))
                .ReturnsAsync((MiseAndPlaceListadoDominio)null);

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(1, 10, 5, DateOnly.FromDateTime(DateTime.Today), 1));

            Assert.Equal("Mise And Place no encontrado.", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_DatosValidos_DescuentaLaRecetaMultiplicadaDirectoPorLaCantidad()
        {
            // Preparar
            var miseAndPlace = CrearMiseAndPlaceExistente();
            var fechaVencimiento = DateOnly.FromDateTime(DateTime.Today.AddDays(30));

            _miseAndPlaceRepoMock.Setup(r => r.ObtenerPorIdAsync(1, 10)).ReturnsAsync(miseAndPlace);
            _generadorNombreLoteServicioMock
                .Setup(s => s.GenerarNombreUnicoAsync(miseAndPlace.Nombre))
                .ReturnsAsync("SALSA-BASE-001");
            _miseAndPlaceRepoMock
                .Setup(r => r.ProducirMiseAndPlaceAsync(1, 10, 5, fechaVencimiento, 2, "SALSA-BASE-001"))
                .ReturnsAsync(99);

            // Ejecutar
            var loteId = await _casoDeUso.EjecutarAsync(1, 10, 5, fechaVencimiento, 2);

            // Verificar
            Assert.Equal(99, loteId);
            _gestionStockServicioMock.Verify(
                s => s.DescontarStockInsumosAsync(
                    1,
                    It.Is<Dictionary<int, decimal>>(d => d[1] == 10 && d[2] == 15)),
                Times.Once);
            _miseAndPlaceRepoMock.Verify(
                r => r.ProducirMiseAndPlaceAsync(1, 10, 5, fechaVencimiento, 2, "SALSA-BASE-001"),
                Times.Once);
        }
    }
}
