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
    public class ModificarMiseAndPlaceCasoDeUsoTest
    {
        private readonly Mock<IMiseAndPlaceRepositorio> _miseAndPlaceRepoMock;
        private readonly Mock<IInsumoValidacionServicio> _insumoValidacionServicioMock;
        private readonly ModificarMiseAndPlaceCasoDeUso _casoDeUso;

        public ModificarMiseAndPlaceCasoDeUsoTest()
        {
            _miseAndPlaceRepoMock = new Mock<IMiseAndPlaceRepositorio>();
            _insumoValidacionServicioMock = new Mock<IInsumoValidacionServicio>();
            _casoDeUso = new ModificarMiseAndPlaceCasoDeUso(
                _miseAndPlaceRepoMock.Object,
                _insumoValidacionServicioMock.Object);
        }

        private static ModificarMiseAndPlaceDominio CrearDatosValidos()
        {
            return new ModificarMiseAndPlaceDominio
            {
                Nombre = "Salsa Base Editada",
                Descripcion = "Salsa para pastas, receta ajustada",
                UnidadMedidaId = 1,
                CategoriaId = 1,
                StockMinimo = 3,
                StockRecomendado = 6,
                Ingredientes = new List<IngredienteDeMiseAndPlace>
                {
                    new IngredienteDeMiseAndPlace { IngredienteId = 1, Cantidad = 1.5m }
                }
            };
        }

        [Fact]
        public async Task EjecutarAsync_IngredientesDuplicados_LanzaArgumentException()
        {
            // Preparar
            var datos = CrearDatosValidos();
            datos.Ingredientes = new List<IngredienteDeMiseAndPlace>
            {
                new IngredienteDeMiseAndPlace { IngredienteId = 1, Cantidad = 1 },
                new IngredienteDeMiseAndPlace { IngredienteId = 1, Cantidad = 2 }
            };

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(1, 10, datos));

            Assert.Equal("Un ingrediente preparado no puede contener el mismo ingrediente más de una vez.", excepcion.Message);
            _miseAndPlaceRepoMock.Verify(
                r => r.ModificarMiseAndPlaceAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ModificarMiseAndPlaceDominio>()),
                Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_InsumosDeLaRecetaInactivos_PropagaLaExcepcion()
        {
            // Preparar
            var datos = CrearDatosValidos();

            _insumoValidacionServicioMock
                .Setup(s => s.ValidarInsumosActivosAsync(It.IsAny<List<int>>(), 1))
                .ThrowsAsync(new ArgumentException("Uno o más insumos de la receta no existen o han sido eliminados."));

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(1, 10, datos));

            Assert.Equal("Uno o más insumos de la receta no existen o han sido eliminados.", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_MiseAndPlaceNoExiste_DevuelveFalse()
        {
            // Preparar
            var datos = CrearDatosValidos();

            _miseAndPlaceRepoMock
                .Setup(r => r.ModificarMiseAndPlaceAsync(1, 10, datos))
                .ReturnsAsync(false);

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(1, 10, datos);

            // Verificar
            Assert.False(resultado);
        }

        [Fact]
        public async Task EjecutarAsync_DatosValidos_ActualizaLaRecetaYDevuelveTrue()
        {
            // Preparar
            var datos = CrearDatosValidos();

            _miseAndPlaceRepoMock
                .Setup(r => r.ModificarMiseAndPlaceAsync(1, 10, datos))
                .ReturnsAsync(true);

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(1, 10, datos);

            // Verificar
            Assert.True(resultado);
            _insumoValidacionServicioMock.Verify(
                s => s.ValidarInsumosActivosAsync(It.Is<List<int>>(ids => ids.Count == 1 && ids.Contains(1)), 1),
                Times.Once);
            _miseAndPlaceRepoMock.Verify(r => r.ModificarMiseAndPlaceAsync(1, 10, datos), Times.Once);
        }
    }
}
