using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace PanComido.Tests.Dominio.CasosDeUso.Insumos
{
    public class CrearInsumoCasoDeUsoTest
    {
        private readonly Mock<IInsumoRepositorio> _insumoRepoMock;
        private readonly Mock<ILoteRepositorio> _loteRepoMock;
        private readonly Mock<IBodegaRepositorio> _bodegaRepoMock;
        private readonly Mock<IInsumoValidacionServicio> _insumoValidacionServicioMock;
        private readonly Mock<IEstadoStockInsumoServicio> _estadoStockServicioMock;
        private readonly Mock<IImagenServicio> _imagenServicioMock;
        private readonly Mock<INormalizadorNombreServicio> _normalizadorNombreServicioMock;
        private readonly Mock<ILogger<CrearInsumoCasoDeUso>> _loggerMock;

        public CrearInsumoCasoDeUsoTest()
        {
            _insumoRepoMock = new Mock<IInsumoRepositorio>();
            _loteRepoMock = new Mock<ILoteRepositorio>();
            _bodegaRepoMock = new Mock<IBodegaRepositorio>();
            _insumoValidacionServicioMock = new Mock<IInsumoValidacionServicio>();
            _estadoStockServicioMock = new Mock<IEstadoStockInsumoServicio>();
            _imagenServicioMock = new Mock<IImagenServicio>();
            _normalizadorNombreServicioMock = new Mock<INormalizadorNombreServicio>();
            _normalizadorNombreServicioMock.Setup(s => s.Normalizar(It.IsAny<string>())).Returns((string nombre) => nombre);
            _loggerMock = new Mock<ILogger<CrearInsumoCasoDeUso>>();
        }

        private CrearInsumoCasoDeUso CrearCasoDeUsoConReposMock()
        {
            return new CrearInsumoCasoDeUso(
                _insumoRepoMock.Object,
                _loteRepoMock.Object,
                _bodegaRepoMock.Object,
                _insumoValidacionServicioMock.Object,
                _estadoStockServicioMock.Object,
                _imagenServicioMock.Object,
                _normalizadorNombreServicioMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTodosLosDatosSonValidos_CreaInsumoConSuLote()
        {
            // 1. Preparar
            CrearInsumoCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int restauranteId = 1;
            int bodegaId = 5;
            int cantidadInicial = 20;

            DateOnly fechaVencimientoFutura = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10);

            Insumo insumo = new Insumo { Nombre = "Harina", CategoriaId = 1, UnidadDeMedidaId = 1, StockMinimo = 5 };

            CategoriaInsumo categoriaValida = new CategoriaInsumo { Id = 1, TipoAplica = TipoInsumo.Ingrediente, Descripcion = "Secos" };
            UnidadMedida unidadValida = new UnidadMedida { Id = 1, Nombre = "Kilos" };

            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarCategoriaAsync(insumo.CategoriaId)).ReturnsAsync(categoriaValida);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarUnidadMedidaAsync(insumo.UnidadDeMedidaId)).ReturnsAsync(unidadValida);
            _bodegaRepoMock.Setup(r => r.ExisteBodegaEnRestauranteAsync(restauranteId, bodegaId)).ReturnsAsync(true);
            _insumoRepoMock.Setup(r => r.CrearAsync(It.IsAny<Insumo>())).ReturnsAsync(insumo);

            _estadoStockServicioMock.Setup(s => s.CalcularEstadoStock(cantidadInicial, insumo.StockMinimo, It.IsAny<decimal>())).Returns(EstadoStock.Normal);

            // 2. Ejecutar
            await casoDeUso.EjecutarAsync(restauranteId, insumo, cantidadInicial, bodegaId, fechaVencimientoFutura, Stream.Null, "", "");

            // 3. Verificar

            // entidad insumo
            Assert.Equal(restauranteId, insumo.RestauranteId);
            Assert.Equal(TipoInsumo.Ingrediente, insumo.Tipo);
            Assert.Equal("Kilos", insumo.UnidadMedida);

            // entidad lote inicial
            Assert.Single(insumo.Lotes);
            Assert.Contains("Lote Harina -", insumo.Lotes[0].Nombre);
            Assert.Equal(cantidadInicial, insumo.Lotes[0].Cantidad);

            // se llego a llamar al repo para el alta
            _insumoRepoMock.Verify(r => r.CrearAsync(insumo), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElNombreYaExiste_LanzaArgumentException()
        {
            // 1. Preparar
            int restauranteId = 1;
            int bodegaId = 5;
            int cantidadInicial = 20;

            CrearInsumoCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            Insumo insumo = new Insumo { Nombre = "Harina", CategoriaId = 1, UnidadDeMedidaId = 1, StockMinimo = 5 };

            _insumoRepoMock.Setup(r => r.ExisteInsumoConNombreAsync(restauranteId, insumo.Nombre)).ReturnsAsync(true);

            // 2. Ejecutar y 3.Verificar que lanza la excepcion
            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                casoDeUso.EjecutarAsync(restauranteId, insumo, cantidadInicial, bodegaId, DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5), Stream.Null, "", ""));

            Assert.Equal($"Ya existe un insumo con el nombre '{insumo.Nombre}' en el restaurante.", excepcion.Message);

            // nunca llega a validar bodega ni a llamar al repositorio de creacion
            _bodegaRepoMock.Verify(r => r.ExisteBodegaEnRestauranteAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _insumoRepoMock.Verify(r => r.CrearAsync(It.IsAny<Insumo>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoCategoriaNoExiste_LanzaArgumentException()
        {
            // 1. Preparar
            int restauranteId = 1;
            int bodegaId = 5;
            int cantidadInicial = 20;

            CrearInsumoCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();

            int categoriaInexistenteId = 99;
            Insumo insumo = new Insumo { CategoriaId = categoriaInexistenteId };

            _bodegaRepoMock.Setup(r => r.ExisteBodegaEnRestauranteAsync(restauranteId, bodegaId)).ReturnsAsync(true);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarCategoriaAsync(categoriaInexistenteId))
                .ThrowsAsync(new ArgumentException("La categoría de insumo seleccionada no existe en el sistema."));

            // 2. Ejecutar y 3.Verificar que lanza la excepcion
            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                casoDeUso.EjecutarAsync(restauranteId, insumo, cantidadInicial, bodegaId, DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5), Stream.Null, "", ""));

            Assert.Equal("La categoría de insumo seleccionada no existe en el sistema.", excepcion.Message);

            // nunca llega a llamar al repositorio
            _insumoRepoMock.Verify(r => r.CrearAsync(It.IsAny<Insumo>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoUnidadDeMedidaNoExiste_LanzaArgumentException()
        {
            // 1. Preparar
            int restauranteId = 1;
            int bodegaId = 5;
            int cantidadInicial = 20;

            CrearInsumoCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int unidadDeMedidaInexistenteId = 99;
            Insumo insumo = new Insumo { CategoriaId = 1, UnidadDeMedidaId = unidadDeMedidaInexistenteId };

            _bodegaRepoMock.Setup(r => r.ExisteBodegaEnRestauranteAsync(restauranteId, bodegaId)).ReturnsAsync(true);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarCategoriaAsync(1)).ReturnsAsync(new CategoriaInsumo());

            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarUnidadMedidaAsync(unidadDeMedidaInexistenteId))
                .ThrowsAsync(new ArgumentException("La unidad de medida seleccionada no existe en el sistema."));

            // 2. Ejecutar y 3.Verificar que lanza la excepcion
            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                casoDeUso.EjecutarAsync(restauranteId, insumo, cantidadInicial, bodegaId, DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5), Stream.Null, "", ""));

            Assert.Equal("La unidad de medida seleccionada no existe en el sistema.", excepcion.Message);

            // nunca llega a llamar al repositorio
            _insumoRepoMock.Verify(r => r.CrearAsync(It.IsAny<Insumo>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoBodegaNoEsDelRestaurante_LanzaArgumentException()
        {
            // 1. Preparar
            int restauranteId = 1;
            int bodegaIdFalsa = 99;
            int cantidadInicial = 20;

            CrearInsumoCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            Insumo insumo = new Insumo { CategoriaId = 1, UnidadDeMedidaId = 1 };

            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarCategoriaAsync(1)).ReturnsAsync(new CategoriaInsumo());
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarUnidadMedidaAsync(1)).ReturnsAsync(new UnidadMedida());
            _bodegaRepoMock.Setup(r => r.ExisteBodegaEnRestauranteAsync(restauranteId, bodegaIdFalsa)).ReturnsAsync(false);

            // 2. Ejecutar y 3.Verificar que lanza la excepcion
            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                casoDeUso.EjecutarAsync(restauranteId, insumo, cantidadInicial, bodegaIdFalsa, DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5), Stream.Null, "", ""));

            Assert.Equal("La bodega destino especificada no es valida o no existe.", excepcion.Message);

            // nunca llega a llamar al repositorio
            _insumoRepoMock.Verify(r => r.CrearAsync(It.IsAny<Insumo>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoCantidadInicialEsMenorAlStockMinimo_LanzaArgumentException()
        {
            // 1. Preparar
            int restauranteId = 1;
            int cantidadInicial = 20;
            int cantidadStockMinimo = 50;
            int bodegaId = 1;

            CrearInsumoCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            Insumo insumo = new Insumo { CategoriaId = 1, UnidadDeMedidaId = 1, StockMinimo = cantidadStockMinimo };

            // 2. Ejecutar y 3.Verificar que lanza la excepcion
            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                casoDeUso.EjecutarAsync(restauranteId, insumo, cantidadInicial, bodegaId, DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5), Stream.Null, "", ""));

            Assert.Equal("La cantidad inicial (20) no puede ser menor al stock mínimo configurado (50).", excepcion.Message);

            // nunca llega a llamar al repositorio
            _insumoRepoMock.Verify(r => r.CrearAsync(It.IsAny<Insumo>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoFechaVencimientoEsPasadaOHoy_LanzaArgumentException()
        {
            // 1. Preparar
            int restauranteId = 1;
            int cantidadInicial = 20;
            int bodegaId = 1;
            DateOnly fechaInvalida = DateOnly.FromDateTime(DateTime.UtcNow); // la misma fecha se considera invalida

            CrearInsumoCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            Insumo insumo = new Insumo { CategoriaId = 1, UnidadDeMedidaId = 1, StockMinimo = 5 };

            // 2. Ejecutar y 3.Verificar que lanza la excepcion
            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                casoDeUso.EjecutarAsync(restauranteId, insumo, cantidadInicial, bodegaId, fechaInvalida, Stream.Null, "", ""));

            Assert.Equal("La fecha de vencimiento debe ser una fecha futura.", excepcion.Message);

            // nunca llega a llamar al repositorio
            _insumoRepoMock.Verify(r => r.CrearAsync(It.IsAny<Insumo>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEsBebidaSinImagen_LanzaArgumentException()
        {
            // 1. Preparar
            int restauranteId = 1;
            int bodegaId = 5;
            int cantidadInicial = 20;

            CrearInsumoCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            Insumo insumo = new Insumo { CategoriaId = 1, UnidadDeMedidaId = 1, StockMinimo = 5 };

            _bodegaRepoMock.Setup(r => r.ExisteBodegaEnRestauranteAsync(restauranteId, bodegaId)).ReturnsAsync(true);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarCategoriaAsync(1)).ReturnsAsync(new CategoriaInsumo { Id = 1, TipoAplica = TipoInsumo.Bebida });
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarUnidadMedidaAsync(1)).ReturnsAsync(new UnidadMedida());

            // 2. Ejecutar y 3.Verificar que lanza la excepcion (sin stream ni nombre de imagen)
            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                casoDeUso.EjecutarAsync(restauranteId, insumo, cantidadInicial, bodegaId, DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5), Stream.Null, "", ""));

            Assert.Equal("La imagen es obligatoria para las bebidas.", excepcion.Message);

            // nunca llega a llamar al repositorio
            _insumoRepoMock.Verify(r => r.CrearAsync(It.IsAny<Insumo>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEsIngredienteSinImagen_NoLanzaExcepcionYNoSubeImagen()
        {
            // 1. Preparar
            int restauranteId = 1;
            int bodegaId = 5;
            int cantidadInicial = 20;

            CrearInsumoCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            Insumo insumo = new Insumo { Nombre = "Harina", CategoriaId = 1, UnidadDeMedidaId = 1, StockMinimo = 5 };

            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarCategoriaAsync(1)).ReturnsAsync(new CategoriaInsumo { Id = 1, TipoAplica = TipoInsumo.Ingrediente, Descripcion = "Secos" });
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarUnidadMedidaAsync(1)).ReturnsAsync(new UnidadMedida { Nombre = "Kilos" });
            _bodegaRepoMock.Setup(r => r.ExisteBodegaEnRestauranteAsync(restauranteId, bodegaId)).ReturnsAsync(true);
            _insumoRepoMock.Setup(r => r.CrearAsync(It.IsAny<Insumo>())).ReturnsAsync(insumo);
            _estadoStockServicioMock.Setup(s => s.CalcularEstadoStock(cantidadInicial, insumo.StockMinimo, It.IsAny<decimal>())).Returns(EstadoStock.Normal);

            // 2. Ejecutar (sin stream ni nombre de imagen — no debería exigirla ni intentar subirla)
            await casoDeUso.EjecutarAsync(restauranteId, insumo, cantidadInicial, bodegaId, DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5), Stream.Null, "", "");

            // 3. Verificar
            Assert.Null(insumo.UrlImagen);
            _imagenServicioMock.Verify(s => s.SubirImagenAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _insumoRepoMock.Verify(r => r.CrearAsync(insumo), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEsIngredienteConEsVisibleEnCartaTrue_LoFuerzaAFalse()
        {
            // 1. Preparar
            int restauranteId = 1;
            int bodegaId = 5;
            int cantidadInicial = 20;

            CrearInsumoCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            Insumo insumo = new Insumo { Nombre = "Harina", CategoriaId = 1, UnidadDeMedidaId = 1, StockMinimo = 5, EsVisibleEnCarta = true };

            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarCategoriaAsync(1)).ReturnsAsync(new CategoriaInsumo { Id = 1, TipoAplica = TipoInsumo.Ingrediente, Descripcion = "Secos" });
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarUnidadMedidaAsync(1)).ReturnsAsync(new UnidadMedida { Nombre = "Kilos" });
            _bodegaRepoMock.Setup(r => r.ExisteBodegaEnRestauranteAsync(restauranteId, bodegaId)).ReturnsAsync(true);
            _insumoRepoMock.Setup(r => r.CrearAsync(It.IsAny<Insumo>())).ReturnsAsync(insumo);

            // 2. Ejecutar
            await casoDeUso.EjecutarAsync(restauranteId, insumo, cantidadInicial, bodegaId, DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5), Stream.Null, "", "");

            // 3. Verificar
            Assert.False(insumo.EsVisibleEnCarta);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEsBebidaConEsVisibleEnCartaTrue_LoRespeta()
        {
            // 1. Preparar
            int restauranteId = 1;
            int bodegaId = 5;
            int cantidadInicial = 20;
            using Stream stream = new MemoryStream();

            CrearInsumoCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            Insumo insumo = new Insumo { Nombre = "Coca", CategoriaId = 1, UnidadDeMedidaId = 1, StockMinimo = 5, EsVisibleEnCarta = true };

            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarCategoriaAsync(1)).ReturnsAsync(new CategoriaInsumo { Id = 1, TipoAplica = TipoInsumo.Bebida, Descripcion = "Sin alcohol" });
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarUnidadMedidaAsync(1)).ReturnsAsync(new UnidadMedida { Nombre = "Unidad" });
            _bodegaRepoMock.Setup(r => r.ExisteBodegaEnRestauranteAsync(restauranteId, bodegaId)).ReturnsAsync(true);
            _insumoRepoMock.Setup(r => r.CrearAsync(It.IsAny<Insumo>())).ReturnsAsync(insumo);

            // 2. Ejecutar
            await casoDeUso.EjecutarAsync(restauranteId, insumo, cantidadInicial, bodegaId, DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5), stream, "foto.jpg", "carpeta");

            // 3. Verificar
            Assert.True(insumo.EsVisibleEnCarta);
        }
    }
}
