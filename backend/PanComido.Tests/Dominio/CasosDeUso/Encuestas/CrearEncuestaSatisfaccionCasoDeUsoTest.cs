using Moq;
using PanComido.Dominio.CasosDeUso.EncuestaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Tests.Dominio.CasosDeUso.Encuestas
{
    public class CrearEncuestaSatisfaccionCasoDeUsoTest
    {
        private readonly Mock<IEncuestaSatisfaccionRepositorio> _encuestaRepoMock;
        private readonly Mock<IComandaRepositorio> _comandaRepoMock;
        private readonly Mock<IRestauranteRepositorio> _restauranteRepoMock;
        private readonly CrearEncuestaSatisfaccionCasoDeUso _casoDeUso;
        public CrearEncuestaSatisfaccionCasoDeUsoTest()
        {
            _encuestaRepoMock = new Mock<IEncuestaSatisfaccionRepositorio>();
            _comandaRepoMock = new Mock<IComandaRepositorio>();
            _restauranteRepoMock = new Mock<IRestauranteRepositorio>();
            _casoDeUso = new CrearEncuestaSatisfaccionCasoDeUso(
                _encuestaRepoMock.Object,
                _comandaRepoMock.Object,
                _restauranteRepoMock.Object);
        }
        [Fact]
        public async Task EjecutarAsync_ComandaNoEncontrada_LanzaKeyNotFoundException()
        {
            // Preparar
            var encuesta = new EncuestaSatisfaccion { ComandaId = 99 };
            _comandaRepoMock.Setup(r => r.ObtenerComandaPorIdAsync(99)).ReturnsAsync((Comanda?)null);
            
            // Ejecutar y Verificar
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _casoDeUso.EjecutarAsync(encuesta));
            Assert.Equal("Comanda no encontrada.", exception.Message);
        }
        [Fact]
        public async Task EjecutarAsync_ComandaNoFinalizada_LanzaArgumentException()
        {
            // Preparar
            var encuesta = new EncuestaSatisfaccion { ComandaId = 1 };
            var comanda = new Comanda { Id = 1, Estado = EstadoComanda.Nueva };

            _comandaRepoMock.Setup(r => r.ObtenerComandaPorIdAsync(1)).ReturnsAsync(comanda);
            
            // Ejecutar y Verificar
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _casoDeUso.EjecutarAsync(encuesta));
            Assert.Equal("La comanda aún no se puede calificar porque no finalizó.", exception.Message);
        }

        [Fact]
        public async Task EjecutarAsync_PromedioMenorA4_GuardaYDevuelveNull()
        {
            // Preparar
            var encuesta = new EncuestaSatisfaccion
            {
                ComandaId = 1,
                PuntuacionLugar = 3,
                PuntuacionComida = 3,
                PuntuacionMozo = 3
            };
            var comanda = new Comanda { Id = 1, Estado = EstadoComanda.Finalizada, RestauranteId = 10 };
            _comandaRepoMock.Setup(r => r.ObtenerComandaPorIdAsync(1)).ReturnsAsync(comanda);
            
            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(encuesta);
            
            // Verificar
            Assert.Null(resultado);
            _encuestaRepoMock.Verify(r => r.GuardarEncuestaAsync(It.IsAny<EncuestaSatisfaccion>()), Times.Once);

            // no se llama a la bd 
            _restauranteRepoMock.Verify(r => r.ObtenerDatosDelLocalAsync(It.IsAny<int>()), Times.Never);
        }
        [Fact]
        public async Task EjecutarAsync_PromedioMayorOIgualA4_GuardaYDevuelveLink()
        {
            // Preparar
            var encuesta = new EncuestaSatisfaccion
            {
                ComandaId = 1,
                PuntuacionLugar = 5,
                PuntuacionComida = 5,
                PuntuacionMozo = 4 // Promedio: 4.66
            };
            var comanda = new Comanda { Id = 1, Estado = EstadoComanda.Finalizada, RestauranteId = 10 };
            var restaurante = new Restaurante { Id = 10, LinkResenaGoogleMaps = "https://maps.app.goo.gl/test" };
            _comandaRepoMock.Setup(r => r.ObtenerComandaPorIdAsync(1)).ReturnsAsync(comanda);
            _restauranteRepoMock.Setup(r => r.ObtenerDatosDelLocalAsync(10)).ReturnsAsync(restaurante);
            
            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(encuesta);
            
            // Verificar
            Assert.Equal("https://maps.app.goo.gl/test", resultado);
            _encuestaRepoMock.Verify(r => r.GuardarEncuestaAsync(It.IsAny<EncuestaSatisfaccion>()), Times.Once);
            _restauranteRepoMock.Verify(r => r.ObtenerDatosDelLocalAsync(10), Times.Once);
        }
    }
}
