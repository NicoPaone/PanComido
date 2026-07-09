using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Insumos
{
    public class ModificarInsumoCasoDeUsoTest
    {
        private readonly Mock<IInsumoRepositorio> _insumoRepoMock;
        private readonly Mock<IEstadoStockInsumoServicio> _estadoStockServicioMock;
        private readonly Mock<IImagenServicio> _imagenServicioMock;
        private readonly Mock<IInsumoValidacionServicio> _insumoValidacionServicioMock;
        private readonly Mock<INormalizadorNombreServicio> _normalizadorNombreServicioMock;
        private readonly Mock<ILogger<ModificarInsumoCasoDeUso>> _loggerMock;
        private readonly ModificarInsumoCasoDeUso _casoDeUso;

        public ModificarInsumoCasoDeUsoTest()
        {
            _insumoRepoMock = new Mock<IInsumoRepositorio>();
            _estadoStockServicioMock = new Mock<IEstadoStockInsumoServicio>();
            _imagenServicioMock = new Mock<IImagenServicio>();
            _insumoValidacionServicioMock = new Mock<IInsumoValidacionServicio>();
            _normalizadorNombreServicioMock = new Mock<INormalizadorNombreServicio>();
            _normalizadorNombreServicioMock.Setup(s => s.Normalizar(It.IsAny<string>())).Returns((string nombre) => nombre);
            _loggerMock = new Mock<ILogger<ModificarInsumoCasoDeUso>>();

            _casoDeUso = new ModificarInsumoCasoDeUso(
                _insumoRepoMock.Object,
                _estadoStockServicioMock.Object,
                _imagenServicioMock.Object,
                _insumoValidacionServicioMock.Object,
                _normalizadorNombreServicioMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoInsumoNoExiste_LanzaArgumentException()
        {
            int restauranteId = 1;
            Insumo insumoModificado = new Insumo { Id = 10, CategoriaId = 1, UnidadDeMedidaId = 1 };

            _insumoRepoMock.Setup(r => r.ObtenerPorIdAsync(insumoModificado.Id, restauranteId))
                .ReturnsAsync((Insumo)null);

            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(restauranteId, insumoModificado, Stream.Null, "", ""));

            Assert.Equal("El insumo que intenta modificar no existe o no pertenece al restaurante.", excepcion.Message);

            _insumoValidacionServicioMock.Verify(s => s.ObtenerYValidarCategoriaAsync(It.IsAny<int>()), Times.Never);
            _insumoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Insumo>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoCategoriaNoCoincideConTipoActual_LanzaArgumentException()
        {
            int restauranteId = 1;
            Insumo insumoModificado = new Insumo { Id = 10, CategoriaId = 2, UnidadDeMedidaId = 1 };
            Insumo insumoExistenteDb = new Insumo { Id = 10, RestauranteId = restauranteId, Tipo = TipoInsumo.Ingrediente };
            CategoriaInsumo categoriaBebida = new CategoriaInsumo { Id = 2, TipoAplica = TipoInsumo.Bebida };

            _insumoRepoMock.Setup(r => r.ObtenerPorIdAsync(insumoModificado.Id, restauranteId))
                .ReturnsAsync(insumoExistenteDb);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarCategoriaAsync(2))
                .ReturnsAsync(categoriaBebida);

            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(restauranteId, insumoModificado, Stream.Null, "", ""));

            Assert.Equal("La categoría seleccionada no es válida para el tipo de insumo especificado.", excepcion.Message);

            _insumoValidacionServicioMock.Verify(s => s.ObtenerYValidarUnidadMedidaAsync(It.IsAny<int>()), Times.Never);
            _insumoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Insumo>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElNuevoNombreYaLoUsaOtroInsumo_LanzaArgumentException()
        {
            int restauranteId = 1;
            Insumo insumoModificado = new Insumo { Id = 10, Nombre = "Azucar", CategoriaId = 1, UnidadDeMedidaId = 1 };
            Insumo insumoExistenteDb = new Insumo { Id = 10, RestauranteId = restauranteId, Nombre = "Harina", Tipo = TipoInsumo.Ingrediente };
            CategoriaInsumo categoria = new CategoriaInsumo { Id = 1, TipoAplica = TipoInsumo.Ingrediente };

            _insumoRepoMock.Setup(r => r.ObtenerPorIdAsync(insumoModificado.Id, restauranteId))
                .ReturnsAsync(insumoExistenteDb);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarCategoriaAsync(1)).ReturnsAsync(categoria);
            _insumoRepoMock.Setup(r => r.ExisteInsumoConNombreAsync(restauranteId, insumoModificado.Nombre)).ReturnsAsync(true);

            ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(() =>
                _casoDeUso.EjecutarAsync(restauranteId, insumoModificado, Stream.Null, "", ""));

            Assert.Equal($"Ya existe un insumo con el nombre '{insumoModificado.Nombre}' en el restaurante.", excepcion.Message);

            _insumoValidacionServicioMock.Verify(s => s.ObtenerYValidarUnidadMedidaAsync(It.IsAny<int>()), Times.Never);
            _insumoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Insumo>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoElNombreNoCambia_NoConsultaDuplicados()
        {
            int restauranteId = 1;
            Insumo insumoModificado = new Insumo { Id = 10, Nombre = "Harina", CategoriaId = 1, UnidadDeMedidaId = 1, StockMinimo = 5, StockRecomendado = 20 };
            Insumo insumoExistenteDb = new Insumo { Id = 10, RestauranteId = restauranteId, Nombre = "Harina", Tipo = TipoInsumo.Ingrediente, StockActual = 30 };
            CategoriaInsumo categoria = new CategoriaInsumo { Id = 1, TipoAplica = TipoInsumo.Ingrediente, Descripcion = "Secos" };
            UnidadMedida unidadMedida = new UnidadMedida { Id = 1, Nombre = "Kilos" };

            _insumoRepoMock.Setup(r => r.ObtenerPorIdAsync(insumoModificado.Id, restauranteId))
                .ReturnsAsync(insumoExistenteDb);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarCategoriaAsync(1)).ReturnsAsync(categoria);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarUnidadMedidaAsync(1)).ReturnsAsync(unidadMedida);
            _estadoStockServicioMock.Setup(s => s.CalcularEstadoStock(30, 5, 20)).Returns(EstadoStock.Normal);

            await _casoDeUso.EjecutarAsync(restauranteId, insumoModificado, Stream.Null, "", "");

            _insumoRepoMock.Verify(r => r.ExisteInsumoConNombreAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _insumoRepoMock.Verify(r => r.ActualizarAsync(insumoExistenteDb), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEsIngredienteValido_ActualizaPropiedadesIncluyendoEsPrecioManual()
        {
            int restauranteId = 1;
            Insumo insumoModificado = new Insumo
            {
                Id = 10,
                Nombre = "Harina Editada",
                Descripcion = "0000",
                PrecioVentaFinal = null,
                EsPrecioManual = true,
                StockMinimo = 5,
                StockRecomendado = 20,
                CategoriaId = 1,
                UnidadDeMedidaId = 1
            };
            Insumo insumoExistenteDb = new Insumo
            {
                Id = 10,
                RestauranteId = restauranteId,
                Nombre = "Harina Vieja",
                EsPrecioManual = false,
                Tipo = TipoInsumo.Ingrediente,
                StockActual = 30
            };
            CategoriaInsumo categoria = new CategoriaInsumo { Id = 1, TipoAplica = TipoInsumo.Ingrediente, Descripcion = "Secos" };
            UnidadMedida unidadMedida = new UnidadMedida { Id = 1, Nombre = "Kilos" };

            _insumoRepoMock.Setup(r => r.ObtenerPorIdAsync(insumoModificado.Id, restauranteId))
                .ReturnsAsync(insumoExistenteDb);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarCategoriaAsync(1)).ReturnsAsync(categoria);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarUnidadMedidaAsync(1)).ReturnsAsync(unidadMedida);
            _estadoStockServicioMock.Setup(s => s.CalcularEstadoStock(30, 5, 20)).Returns(EstadoStock.Normal);

            Insumo resultado = await _casoDeUso.EjecutarAsync(restauranteId, insumoModificado, Stream.Null, "", "");

            Assert.Equal("Harina Editada", insumoExistenteDb.Nombre);
            Assert.True(insumoExistenteDb.EsPrecioManual);
            Assert.Null(insumoExistenteDb.UrlImagen);
            Assert.Equal("Secos", resultado.Categoria);
            Assert.Equal("Kilos", resultado.UnidadMedida);
            Assert.Equal(EstadoStock.Normal, resultado.EstadoStock);

            _insumoRepoMock.Verify(r => r.ActualizarAsync(insumoExistenteDb), Times.Once);
            _imagenServicioMock.Verify(s => s.SubirImagenAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEsBebidaConImagenNueva_SubeImagenYActualizaUrl()
        {
            int restauranteId = 1;
            Insumo insumoModificado = new Insumo { Id = 10, CategoriaId = 1, UnidadDeMedidaId = 1, StockMinimo = 1, StockRecomendado = 2 };
            Insumo insumoExistenteDb = new Insumo
            {
                Id = 10,
                RestauranteId = restauranteId,
                Tipo = TipoInsumo.Bebida,
                UrlImagen = "url-vieja.jpg"
            };
            CategoriaInsumo categoria = new CategoriaInsumo { Id = 1, TipoAplica = TipoInsumo.Bebida, Descripcion = "Gaseosas" };
            UnidadMedida unidadMedida = new UnidadMedida { Id = 1, Nombre = "Unidad" };
            using Stream stream = new MemoryStream();

            _insumoRepoMock.Setup(r => r.ObtenerPorIdAsync(insumoModificado.Id, restauranteId))
                .ReturnsAsync(insumoExistenteDb);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarCategoriaAsync(1)).ReturnsAsync(categoria);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarUnidadMedidaAsync(1)).ReturnsAsync(unidadMedida);
            _imagenServicioMock.Setup(s => s.SubirImagenAsync(stream, "foto.jpg", "carpeta"))
                .ReturnsAsync("url-nueva.jpg");

            await _casoDeUso.EjecutarAsync(restauranteId, insumoModificado, stream, "foto.jpg", "carpeta");

            Assert.Equal("url-nueva.jpg", insumoExistenteDb.UrlImagen);
            _imagenServicioMock.Verify(s => s.SubirImagenAsync(stream, "foto.jpg", "carpeta"), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEsBebidaSinImagenNueva_ConservaUrlImagenExistente()
        {
            int restauranteId = 1;
            Insumo insumoModificado = new Insumo { Id = 10, CategoriaId = 1, UnidadDeMedidaId = 1, StockMinimo = 1, StockRecomendado = 2 };
            Insumo insumoExistenteDb = new Insumo
            {
                Id = 10,
                RestauranteId = restauranteId,
                Tipo = TipoInsumo.Bebida,
                UrlImagen = "url-existente.jpg"
            };
            CategoriaInsumo categoria = new CategoriaInsumo { Id = 1, TipoAplica = TipoInsumo.Bebida, Descripcion = "Gaseosas" };
            UnidadMedida unidadMedida = new UnidadMedida { Id = 1, Nombre = "Unidad" };

            _insumoRepoMock.Setup(r => r.ObtenerPorIdAsync(insumoModificado.Id, restauranteId))
                .ReturnsAsync(insumoExistenteDb);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarCategoriaAsync(1)).ReturnsAsync(categoria);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarUnidadMedidaAsync(1)).ReturnsAsync(unidadMedida);

            // Edición sin adjuntar una foto nueva: no debe pisar ni subir nada
            await _casoDeUso.EjecutarAsync(restauranteId, insumoModificado, Stream.Null, "", "carpeta");

            Assert.Equal("url-existente.jpg", insumoExistenteDb.UrlImagen);
            _imagenServicioMock.Verify(s => s.SubirImagenAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEsIngrediente_UrlImagenQuedaNulaAunqueLlegueUnaImagen()
        {
            int restauranteId = 1;
            Insumo insumoModificado = new Insumo { Id = 10, CategoriaId = 1, UnidadDeMedidaId = 1, StockMinimo = 1, StockRecomendado = 2 };
            Insumo insumoExistenteDb = new Insumo
            {
                Id = 10,
                RestauranteId = restauranteId,
                Tipo = TipoInsumo.Ingrediente,
                UrlImagen = null
            };
            CategoriaInsumo categoria = new CategoriaInsumo { Id = 1, TipoAplica = TipoInsumo.Ingrediente, Descripcion = "Secos" };
            UnidadMedida unidadMedida = new UnidadMedida { Id = 1, Nombre = "Kilos" };
            using Stream stream = new MemoryStream();

            _insumoRepoMock.Setup(r => r.ObtenerPorIdAsync(insumoModificado.Id, restauranteId))
                .ReturnsAsync(insumoExistenteDb);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarCategoriaAsync(1)).ReturnsAsync(categoria);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarUnidadMedidaAsync(1)).ReturnsAsync(unidadMedida);

            // Aunque llegue un stream/nombreImagen (por error del front), un Ingrediente lo ignora
            await _casoDeUso.EjecutarAsync(restauranteId, insumoModificado, stream, "foto.jpg", "carpeta");

            Assert.Null(insumoExistenteDb.UrlImagen);
            _imagenServicioMock.Verify(s => s.SubirImagenAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEsIngredienteConEsVisibleEnCartaTrue_LoFuerzaAFalse()
        {
            int restauranteId = 1;
            Insumo insumoModificado = new Insumo { Id = 10, CategoriaId = 1, UnidadDeMedidaId = 1, StockMinimo = 1, StockRecomendado = 2, EsVisibleEnCarta = true };
            Insumo insumoExistenteDb = new Insumo { Id = 10, RestauranteId = restauranteId, Tipo = TipoInsumo.Ingrediente };
            CategoriaInsumo categoria = new CategoriaInsumo { Id = 1, TipoAplica = TipoInsumo.Ingrediente, Descripcion = "Secos" };
            UnidadMedida unidadMedida = new UnidadMedida { Id = 1, Nombre = "Kilos" };

            _insumoRepoMock.Setup(r => r.ObtenerPorIdAsync(insumoModificado.Id, restauranteId))
                .ReturnsAsync(insumoExistenteDb);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarCategoriaAsync(1)).ReturnsAsync(categoria);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarUnidadMedidaAsync(1)).ReturnsAsync(unidadMedida);

            await _casoDeUso.EjecutarAsync(restauranteId, insumoModificado, Stream.Null, "", "");

            Assert.False(insumoExistenteDb.EsVisibleEnCarta);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoEsBebidaConEsVisibleEnCartaTrue_LoRespeta()
        {
            int restauranteId = 1;
            Insumo insumoModificado = new Insumo { Id = 10, CategoriaId = 1, UnidadDeMedidaId = 1, StockMinimo = 1, StockRecomendado = 2, EsVisibleEnCarta = true };
            Insumo insumoExistenteDb = new Insumo { Id = 10, RestauranteId = restauranteId, Tipo = TipoInsumo.Bebida, UrlImagen = "url.jpg" };
            CategoriaInsumo categoria = new CategoriaInsumo { Id = 1, TipoAplica = TipoInsumo.Bebida, Descripcion = "Sin alcohol" };
            UnidadMedida unidadMedida = new UnidadMedida { Id = 1, Nombre = "Unidad" };

            _insumoRepoMock.Setup(r => r.ObtenerPorIdAsync(insumoModificado.Id, restauranteId))
                .ReturnsAsync(insumoExistenteDb);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarCategoriaAsync(1)).ReturnsAsync(categoria);
            _insumoValidacionServicioMock.Setup(s => s.ObtenerYValidarUnidadMedidaAsync(1)).ReturnsAsync(unidadMedida);

            await _casoDeUso.EjecutarAsync(restauranteId, insumoModificado, Stream.Null, "", "carpeta");

            Assert.True(insumoExistenteDb.EsVisibleEnCarta);
        }
    }
}
