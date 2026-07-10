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
    public class CrearMiseAndPlaceCasoDeUsoTest
    {
        private readonly Mock<IMiseAndPlaceRepositorio> _miseAndPlaceRepoMock;
        private readonly Mock<IInsumoValidacionServicio> _insumoValidacionServicioMock;
        private readonly Mock<IInsumoRepositorio> _insumoRepoMock;
        private readonly CrearMiseAndPlaceCasoDeUso _casoDeUso;

        public CrearMiseAndPlaceCasoDeUsoTest()
        {
            _miseAndPlaceRepoMock = new Mock<IMiseAndPlaceRepositorio>();
            _insumoValidacionServicioMock = new Mock<IInsumoValidacionServicio>();
            _insumoRepoMock = new Mock<IInsumoRepositorio>();
            _casoDeUso = new CrearMiseAndPlaceCasoDeUso(
                _miseAndPlaceRepoMock.Object,
                _insumoValidacionServicioMock.Object,
                _insumoRepoMock.Object);
        }

        private static NuevoMiseAndPlace CrearNuevoMiseAndPlaceValido()
        {
            return new NuevoMiseAndPlace
            {
                Nombre = "Salsa Base",
                Descripcion = "Salsa para pastas",
                UnidadMedidaId = 1,
                CategoriaId = 1,
                StockMinimo = 2,
                StockRecomendado = 5,
                RestauranteId = 1,
                Ingredientes = new List<IngredienteDeMiseAndPlace>
                {
                    new IngredienteDeMiseAndPlace { IngredienteId = 1, Cantidad = 1 },
                    new IngredienteDeMiseAndPlace { IngredienteId = 2, Cantidad = 2 }
                }
            };
        }

        [Fact]
        public async Task EjecutarAsync_IngredientesDuplicados_LanzaArgumentException()
        {
            // Preparar
            var nuevoMiseAndPlace = CrearNuevoMiseAndPlaceValido();
            nuevoMiseAndPlace.Ingredientes = new List<IngredienteDeMiseAndPlace>
            {
                new IngredienteDeMiseAndPlace { IngredienteId = 1, Cantidad = 1 },
                new IngredienteDeMiseAndPlace { IngredienteId = 1, Cantidad = 2 }
            };

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(nuevoMiseAndPlace));

            Assert.Equal("Un ingrediente preparado no puede contener el mismo ingrediente más de una vez.", excepcion.Message);
            _miseAndPlaceRepoMock.Verify(r => r.CrearMiseAndPlaceAsync(It.IsAny<NuevoMiseAndPlace>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_InsumosDeLaRecetaInactivos_PropagaLaExcepcion()
        {
            // Preparar
            var nuevoMiseAndPlace = CrearNuevoMiseAndPlaceValido();

            _insumoValidacionServicioMock
                .Setup(s => s.ValidarInsumosActivosAsync(It.IsAny<List<int>>(), nuevoMiseAndPlace.RestauranteId))
                .ThrowsAsync(new ArgumentException("Uno o más insumos de la receta no existen o han sido eliminados."));

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(nuevoMiseAndPlace));

            Assert.Equal("Uno o más insumos de la receta no existen o han sido eliminados.", excepcion.Message);
            _miseAndPlaceRepoMock.Verify(r => r.CrearMiseAndPlaceAsync(It.IsAny<NuevoMiseAndPlace>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_NombreYaExiste_LanzaArgumentException()
        {
            // Preparar
            var nuevoMiseAndPlace = CrearNuevoMiseAndPlaceValido();

            _insumoRepoMock
                .Setup(r => r.ExisteInsumoConNombreAsync(nuevoMiseAndPlace.RestauranteId, nuevoMiseAndPlace.Nombre))
                .ReturnsAsync(true);

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(nuevoMiseAndPlace));

            Assert.Equal("Ese nombre ya existe. Elija otro nombre", excepcion.Message);
            _miseAndPlaceRepoMock.Verify(r => r.CrearMiseAndPlaceAsync(It.IsAny<NuevoMiseAndPlace>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_DatosValidos_CreaLaRecetaSinDescontarStock()
        {
            // Preparar
            var nuevoMiseAndPlace = CrearNuevoMiseAndPlaceValido();

            _insumoRepoMock
                .Setup(r => r.ExisteInsumoConNombreAsync(nuevoMiseAndPlace.RestauranteId, nuevoMiseAndPlace.Nombre))
                .ReturnsAsync(false);
            _miseAndPlaceRepoMock
                .Setup(r => r.CrearMiseAndPlaceAsync(nuevoMiseAndPlace))
                .ReturnsAsync(10);

            // Ejecutar
            var id = await _casoDeUso.EjecutarAsync(nuevoMiseAndPlace);

            // Verificar
            Assert.Equal(10, id);
            _insumoValidacionServicioMock.Verify(
                s => s.ValidarInsumosActivosAsync(
                    It.Is<List<int>>(ids => ids.Count == 2 && ids.Contains(1) && ids.Contains(2)),
                    nuevoMiseAndPlace.RestauranteId),
                Times.Once);
            _miseAndPlaceRepoMock.Verify(r => r.CrearMiseAndPlaceAsync(nuevoMiseAndPlace), Times.Once);
        }
    }
}
