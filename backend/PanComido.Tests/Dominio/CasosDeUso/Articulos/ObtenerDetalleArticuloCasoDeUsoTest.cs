using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.ArticuloCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Tests.Dominio.CasosDeUso.Articulos
{
    public class ObtenerDetalleArticuloCasoDeUsoTest
    {
        private readonly Mock<IArticuloRepositorio> _articuloRepoMock;
        private readonly Mock<ILoteRepositorio> _loteRepoMock;
        private readonly Mock<IDisponibilidadArticuloServicio> _disponibilidadServicioMock;
        private readonly Mock<ILogger<ObtenerDetalleArticuloCasoDeUso>> _loggerMock;

        public ObtenerDetalleArticuloCasoDeUsoTest()
        {
            _articuloRepoMock = new Mock<IArticuloRepositorio>();
            _loteRepoMock = new Mock<ILoteRepositorio>();
            _disponibilidadServicioMock = new Mock<IDisponibilidadArticuloServicio>();
            _loggerMock = new Mock<ILogger<ObtenerDetalleArticuloCasoDeUso>>();
        }

        private ObtenerDetalleArticuloCasoDeUso CrearCasoDeUsoConReposMock()
        {
            return new ObtenerDetalleArticuloCasoDeUso(
                _articuloRepoMock.Object,
                _disponibilidadServicioMock.Object,
                _loteRepoMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTodoEsValido_RetornaArticulo()
        {
            // 1. Preparar
            ObtenerDetalleArticuloCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int restauranteId = 1;
            int articuloId = 4;

            Articulo articuloValido = new Articulo { Id = articuloId, Nombre = "Pizza", EsVisibleEnCarta = true };
            Dictionary<int, decimal> stockDisponible = new Dictionary<int, decimal>();

            _articuloRepoMock.Setup(r => r.ObtenerDetalleAsync(restauranteId, articuloId)).ReturnsAsync(articuloValido);
            _loteRepoMock.Setup(r => r.ObtenerStockTotalDeInsumosDisponible(restauranteId, It.IsAny<DateOnly>())).ReturnsAsync(stockDisponible);
            _disponibilidadServicioMock.Setup(s => s.VerificarDisponibilidad(articuloValido, stockDisponible)).Returns(true);

            // 2. Ejecutar
            Articulo resultado = await casoDeUso.EjecutarAsync(restauranteId, articuloId);

            // 3. Verificar
            Assert.NotNull(resultado);
            Assert.Equal(articuloId, resultado.Id);
            Assert.Equal("Pizza", resultado.Nombre);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoArticuloNoExiste_LanzaArgumentException()
        {
            // 1. Preparar
            ObtenerDetalleArticuloCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int restauranteId = 1;
            int articuloId = 99;

            Articulo articuloNulo = null;

            _articuloRepoMock.Setup(r => r.ObtenerDetalleAsync(restauranteId, articuloId)).ReturnsAsync(articuloNulo);

            // 2. Ejecutar y 3. Verificar que lanza la excepcion
            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                casoDeUso.EjecutarAsync(restauranteId, articuloId));

            Assert.Equal("El artículo no existe o no pertenece al restaurante.", excepcion.Message);

            // nunca llega a consultar stock
            _loteRepoMock.Verify(r => r.ObtenerStockTotalDeInsumosDisponible(It.IsAny<int>(), It.IsAny<DateOnly>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoArticuloNoEsVisibleEnCarta_LanzaArgumentException()
        {
            // 1. Preparar
            ObtenerDetalleArticuloCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int restauranteId = 1;
            int articuloId = 5;

            Articulo articuloOculto = new Articulo { Id = articuloId, EsVisibleEnCarta = false };

            _articuloRepoMock.Setup(r => r.ObtenerDetalleAsync(restauranteId, articuloId)).ReturnsAsync(articuloOculto);

            // 2. Ejecutar y 3. Verificar que lanza la excepcion
            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                casoDeUso.EjecutarAsync(restauranteId, articuloId));

            Assert.Equal("El artículo solicitado no está disponible en la carta.", excepcion.Message);

            // nunca llega a consultar stock
            _loteRepoMock.Verify(r => r.ObtenerStockTotalDeInsumosDisponible(It.IsAny<int>(), It.IsAny<DateOnly>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoHayStockSuficiente_LanzaArgumentException()
        {
            // 1. Preparar
            ObtenerDetalleArticuloCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int restauranteId = 1;
            int articuloId = 10;

            Articulo articuloVisible = new Articulo { Id = articuloId, EsVisibleEnCarta = true };
            Dictionary<int, decimal> stockVacio = new Dictionary<int, decimal>();

            _articuloRepoMock.Setup(r => r.ObtenerDetalleAsync(restauranteId, articuloId)).ReturnsAsync(articuloVisible);
            _loteRepoMock.Setup(r => r.ObtenerStockTotalDeInsumosDisponible(restauranteId, It.IsAny<DateOnly>())).ReturnsAsync(stockVacio);

            _disponibilidadServicioMock.Setup(s => s.VerificarDisponibilidad(articuloVisible, stockVacio)).Returns(false);

            // 2. Ejecutar y 3. Verificar que lanza la excepcion
            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                casoDeUso.EjecutarAsync(restauranteId, articuloId));

            Assert.Equal("El artículo solicitado no está disponible actualmente debido a la falta de insumos.", excepcion.Message);
        }
    }
}
