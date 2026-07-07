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
    public class CrearReglaTiempoExtraCasoDeUsoTests
    {
        private readonly Mock<IReglaTiempoExtraRepositorio> _mockRepo;
        private readonly CrearReglaTiempoExtraCasoDeUso _casoDeUso;

        public CrearReglaTiempoExtraCasoDeUsoTests()
        {
            _mockRepo = new Mock<IReglaTiempoExtraRepositorio>();
            _casoDeUso = new CrearReglaTiempoExtraCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task EjecutarAsync_DeberiaCrearRegla_CuandoPorcentajeNoExiste()
        {
            // Preparar
            var restauranteId = 1;
            var nuevaRegla = new ReglaTiempoExtra { RestauranteId = restauranteId, PorcentajeOcupacionHasta = 50, MinutosExtra = 15 };
            
            _mockRepo.Setup(r => r.ObtenerPorRestauranteIdAsync(restauranteId))
                     .ReturnsAsync(new List<ReglaTiempoExtra>()); // No hay reglas
                     
            _mockRepo.Setup(r => r.CrearAsync(It.IsAny<ReglaTiempoExtra>()))
                     .ReturnsAsync((ReglaTiempoExtra r) => { r.Id = 10; return r; });

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(nuevaRegla);

            // Verificar
            Assert.NotNull(resultado);
            Assert.Equal(10, resultado.Id);
            _mockRepo.Verify(r => r.CrearAsync(nuevaRegla), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_DeberiaLanzarExcepcion_CuandoPorcentajeYaExiste()
        {
            // Preparar
            var restauranteId = 1;
            var nuevaRegla = new ReglaTiempoExtra { RestauranteId = restauranteId, PorcentajeOcupacionHasta = 50, MinutosExtra = 20 };
            var reglaExistente = new ReglaTiempoExtra { Id = 1, RestauranteId = restauranteId, PorcentajeOcupacionHasta = 50, MinutosExtra = 10 };
            
            _mockRepo.Setup(r => r.ObtenerPorRestauranteIdAsync(restauranteId))
                     .ReturnsAsync(new List<ReglaTiempoExtra> { reglaExistente });

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() => _casoDeUso.EjecutarAsync(nuevaRegla));
            Assert.Contains("Ya existe una regla configurada para el 50%", excepcion.Message);
            _mockRepo.Verify(r => r.CrearAsync(It.IsAny<ReglaTiempoExtra>()), Times.Never);
        }
    }
}
