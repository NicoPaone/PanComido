using Moq;
using PanComido.Dominio.CasosDeUso.ReglaTiempoExtraCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.ReglaTiempoExtraCasosDeUso
{
    public class ModificarReglaTiempoExtraCasoDeUsoTests
    {
        private readonly Mock<IReglaTiempoExtraRepositorio> _mockRepo;
        private readonly ModificarReglaTiempoExtraCasoDeUso _casoDeUso;

        public ModificarReglaTiempoExtraCasoDeUsoTests()
        {
            _mockRepo = new Mock<IReglaTiempoExtraRepositorio>();
            _casoDeUso = new ModificarReglaTiempoExtraCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task EjecutarAsync_DeberiaModificarRegla_CuandoDatosSonValidos()
        {
            // Preparar
            var idRegla = 1;
            var restauranteId = 10;
            var reglaExistente = new ReglaTiempoExtra { Id = idRegla, RestauranteId = restauranteId, PorcentajeOcupacionHasta = 30, MinutosExtra = 5 };
            var reglaActualizada = new ReglaTiempoExtra { RestauranteId = restauranteId, PorcentajeOcupacionHasta = 40, MinutosExtra = 10 };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(idRegla)).ReturnsAsync(reglaExistente);
            _mockRepo.Setup(r => r.ObtenerPorRestauranteIdAsync(restauranteId)).ReturnsAsync(new List<ReglaTiempoExtra> { reglaExistente });
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<ReglaTiempoExtra>())).ReturnsAsync((ReglaTiempoExtra r) => r);

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(idRegla, reglaActualizada);

            // Verificar
            Assert.NotNull(resultado);
            Assert.Equal(40, resultado.PorcentajeOcupacionHasta);
            Assert.Equal(10, resultado.MinutosExtra);
            _mockRepo.Verify(r => r.ActualizarAsync(It.IsAny<ReglaTiempoExtra>()), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_DeberiaLanzarExcepcion_CuandoReglaNoExiste()
        {
            // Preparar
            var idRegla = 1;
            var reglaActualizada = new ReglaTiempoExtra { RestauranteId = 10, PorcentajeOcupacionHasta = 40, MinutosExtra = 10 };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(idRegla)).ReturnsAsync((ReglaTiempoExtra)null);

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<KeyNotFoundException>(() => _casoDeUso.EjecutarAsync(idRegla, reglaActualizada));
            Assert.Equal("Regla no encontrada.", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_DeberiaLanzarExcepcion_CuandoReglaPerteneceAOtroRestaurante()
        {
            // Preparar
            var idRegla = 1;
            var reglaExistente = new ReglaTiempoExtra { Id = idRegla, RestauranteId = 999 }; // Otro restaurante
            var reglaActualizada = new ReglaTiempoExtra { RestauranteId = 10 };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(idRegla)).ReturnsAsync(reglaExistente);

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<KeyNotFoundException>(() => _casoDeUso.EjecutarAsync(idRegla, reglaActualizada));
            Assert.Equal("Regla no encontrada.", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_DeberiaLanzarExcepcion_CuandoPorcentajeYaExisteEnOtraRegla()
        {
            // Preparar
            var idRegla = 1;
            var restauranteId = 10;
            var reglaAModificar = new ReglaTiempoExtra { Id = idRegla, RestauranteId = restauranteId, PorcentajeOcupacionHasta = 30 };
            var reglaActualizada = new ReglaTiempoExtra { RestauranteId = restauranteId, PorcentajeOcupacionHasta = 80 };
            
            // Ya existe otra regla (id = 2) con el 80%
            var otraRegla = new ReglaTiempoExtra { Id = 2, RestauranteId = restauranteId, PorcentajeOcupacionHasta = 80 };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(idRegla)).ReturnsAsync(reglaAModificar);
            _mockRepo.Setup(r => r.ObtenerPorRestauranteIdAsync(restauranteId)).ReturnsAsync(new List<ReglaTiempoExtra> { reglaAModificar, otraRegla });

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() => _casoDeUso.EjecutarAsync(idRegla, reglaActualizada));
            Assert.Contains("Ya existe otra regla configurada para el 80%", excepcion.Message);
            _mockRepo.Verify(r => r.ActualizarAsync(It.IsAny<ReglaTiempoExtra>()), Times.Never);
        }
    }
}
