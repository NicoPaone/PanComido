using Moq;
using PanComido.Dominio.CasosDeUso.LoteCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Lotes
{
    public class ModificarLoteCasoDeUsoTest
    {
        private readonly Mock<ILoteRepositorio> _mockLoteRepositorio;
        private readonly Mock<IInsumoRepositorio> _mockInsumoRepositorio;
        private readonly Mock<IGeneradorNombreLoteServicio> _mockGeneradorNombreLote;
        private readonly ModificarLoteCasoDeUso _casoDeUso;

        public ModificarLoteCasoDeUsoTest()
        {
            _mockLoteRepositorio = new Mock<ILoteRepositorio>();
            _mockInsumoRepositorio = new Mock<IInsumoRepositorio>();
            _mockGeneradorNombreLote = new Mock<IGeneradorNombreLoteServicio>();

            _casoDeUso = new ModificarLoteCasoDeUso(
                _mockLoteRepositorio.Object,
                _mockInsumoRepositorio.Object,
                _mockGeneradorNombreLote.Object);
        }

        [Fact]
        public async Task EjecutarAsync_LoteNoExiste_RetornaFalse()
        {
            // Preparar
            int restauranteId = 1;
            int loteId = 10;

            _mockLoteRepositorio.Setup(x => x.ObtenerPorIdAsync(restauranteId, loteId))
                .ReturnsAsync((Lote?)null);

            // Actuar
            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, loteId, 5, 20m, DateOnly.FromDateTime(DateTime.Now), 2);

            // Verificar
            Assert.False(resultado);
        }

        [Fact]
        public async Task EjecutarAsync_NuevoInsumoNoExiste_LanzaExcepcion()
        {
            // Preparar
            int restauranteId = 1;
            int loteId = 10;
            var lote = new Lote { Id = loteId, InsumoId = 2 };

            _mockLoteRepositorio.Setup(x => x.ObtenerPorIdAsync(restauranteId, loteId))
                .ReturnsAsync(lote);

            _mockInsumoRepositorio.Setup(x => x.ObtenerPorIdAsync(5, restauranteId))
                .ReturnsAsync((Insumo?)null);

            // Actuar & Verificar
            var excepcion = await Assert.ThrowsAsync<Exception>(() => 
                _casoDeUso.EjecutarAsync(restauranteId, loteId, 5, 20m, DateOnly.FromDateTime(DateTime.Now), 2));

            Assert.Equal("El nuevo insumo especificado no existe.", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_CambioDeInsumo_ActualizaLoteCorrectamente()
        {
            // Preparar
            int restauranteId = 1;
            int loteId = 10;
            int nuevoInsumoId = 5;
            decimal nuevaCantidad = 30m;
            int nuevaBodegaId = 3;
            var nuevaFecha = DateOnly.FromDateTime(DateTime.Now.AddDays(15));

            var lote = new Lote { Id = loteId, InsumoId = 2, Nombre = "LOTE-VIEJO", Cantidad = 10m };
            var nuevoInsumo = new Insumo { Id = nuevoInsumoId, Nombre = "Nuevo Insumo" };

            _mockLoteRepositorio.Setup(x => x.ObtenerPorIdAsync(restauranteId, loteId))
                .ReturnsAsync(lote);

            _mockInsumoRepositorio.Setup(x => x.ObtenerPorIdAsync(nuevoInsumoId, restauranteId))
                .ReturnsAsync(nuevoInsumo);

            _mockGeneradorNombreLote.Setup(x => x.GenerarNombreUnicoAsync("Nuevo Insumo"))
                .ReturnsAsync("LOTE-NUEVO-123");

            // Actuar
            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, loteId, nuevoInsumoId, nuevaCantidad, nuevaFecha, nuevaBodegaId);

            // Verificar
            Assert.True(resultado);
            Assert.Equal("LOTE-NUEVO-123", lote.Nombre);
            Assert.Equal(nuevoInsumoId, lote.InsumoId);
            Assert.Equal(nuevaCantidad, lote.Cantidad);
            Assert.Equal(nuevaBodegaId, lote.BodegaId);
            Assert.Equal(nuevaFecha, lote.FechaVencimiento);

            _mockLoteRepositorio.Verify(x => x.ActualizarLotesAsync(It.Is<List<Lote>>(l => l.Contains(lote))), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_MismoInsumo_ActualizaCantidadesSinCambiarNombre()
        {
            // Preparar
            int restauranteId = 1;
            int loteId = 10;
            int mismoInsumoId = 2;
            var lote = new Lote { Id = loteId, InsumoId = mismoInsumoId, Nombre = "LOTE-MANTIENE", Cantidad = 10m };

            _mockLoteRepositorio.Setup(x => x.ObtenerPorIdAsync(restauranteId, loteId))
                .ReturnsAsync(lote);

            // Actuar
            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, loteId, mismoInsumoId, 50m, DateOnly.FromDateTime(DateTime.Now), 4);

            // Verificar
            Assert.True(resultado);
            Assert.Equal("LOTE-MANTIENE", lote.Nombre); // No debe cambiar
            Assert.Equal(mismoInsumoId, lote.InsumoId);
            Assert.Equal(50m, lote.Cantidad);
            Assert.Equal(4, lote.BodegaId);

            _mockInsumoRepositorio.Verify(x => x.ObtenerPorIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _mockGeneradorNombreLote.Verify(x => x.GenerarNombreUnicoAsync(It.IsAny<string>()), Times.Never);
            _mockLoteRepositorio.Verify(x => x.ActualizarLotesAsync(It.Is<List<Lote>>(l => l.Contains(lote))), Times.Once);
        }
    }
}
