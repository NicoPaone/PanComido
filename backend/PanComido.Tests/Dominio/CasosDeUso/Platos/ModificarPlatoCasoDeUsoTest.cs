using Moq;
using PanComido.Dominio.CasosDeUso.PlatoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Platos
{
    public class ModificarPlatoCasoDeUsoTest
    {
        private readonly Mock<IPlatoRepositorio> _platoRepoMock;
        private readonly ModificarPlatoCasoDeUso _casoDeUso;

        public ModificarPlatoCasoDeUsoTest()
        {
            _platoRepoMock = new Mock<IPlatoRepositorio>();
            _casoDeUso = new ModificarPlatoCasoDeUso(_platoRepoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoPlatoNoExiste_LanzaArgumentException()
        {
            // Preparar
            int restauranteId = 1;
            var platoModificado = new Plato { Id = 10, Nombre = "Nuevo Nombre" };

            // Simulamos que buscar el plato en la BD devuelve nulo
            _platoRepoMock.Setup(r => r.ObtenerPorIdAsync(platoModificado.Id, restauranteId))
                          .ReturnsAsync((Plato)null);

            // Ejecutar y Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() => 
                _casoDeUso.EjecutarAsync(restauranteId, platoModificado));

            Assert.Equal("El plato que intenta modificar no existe o no pertenece al restaurante.", excepcion.Message);
            
            // Verificamos que el repositorio nunca intentó guardar en BD
            _platoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Plato>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoPlatoExiste_ActualizaPropiedadesYLlamaActualizarAsync()
        {
            // Preparar
            int restauranteId = 1;
            var platoModificado = new Plato { Id = 10, Nombre = "Plato Editado", PrecioVentaFinal = 500 };
            var platoExistenteDb = new Plato { Id = 10, Nombre = "Plato Viejo", PrecioVentaFinal = 200 };

            // Simulamos que el plato existe en BD
            _platoRepoMock.Setup(r => r.ObtenerPorIdAsync(platoModificado.Id, restauranteId))
                          .ReturnsAsync(platoExistenteDb);

            // Ejecutar
            await _casoDeUso.EjecutarAsync(restauranteId, platoModificado);

            // Verificar
            Assert.Equal("Plato Editado", platoExistenteDb.Nombre);
            Assert.Equal(500, platoExistenteDb.PrecioVentaFinal);
            
            // Verificamos que se llamó al método de guardar exactamente una vez
            _platoRepoMock.Verify(r => r.ActualizarAsync(platoExistenteDb), Times.Once);
        }
    }
}
