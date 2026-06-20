using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Configuracion
{
    public class ActualizarPorcentajesCasoDeUsoTest
    {
        private readonly Mock<IPorcentajesCategoriaRepositorio> _porcentajesRepoMock;
        private readonly Mock<ILogger<ActualizarPorcentajesCasoDeUso>> _loggerMock;

        public ActualizarPorcentajesCasoDeUsoTest()
        {
            _porcentajesRepoMock = new Mock<IPorcentajesCategoriaRepositorio>();
            _loggerMock = new Mock<ILogger<ActualizarPorcentajesCasoDeUso>>();
        }

        private ActualizarPorcentajesCasoDeUso CrearCasoDeUso() =>
            new ActualizarPorcentajesCasoDeUso(_porcentajesRepoMock.Object, _loggerMock.Object);

        [Fact]
        public async Task EjecutarAsync_CuandoActualiza_DevuelvePorcentajesActualizados()
        {
            int restauranteId = 1;
            var platos = new List<DOM.PorcentajesCategoria>
            {
                new DOM.PorcentajesCategoria { Id = 1, Descripcion = "Entrada", Porcentaje = 10m }
            };
            var bebidas = new List<DOM.PorcentajesCategoria>
            {
                new DOM.PorcentajesCategoria { Id = 2, Descripcion = "Gaseosa", Porcentaje = 15m }
            };
            var porcentajesActualizados = new DOM.PorcentajesGanancia { Platos = platos, Bebidas = bebidas };

            _porcentajesRepoMock
                .Setup(r => r.ActualizarPorcentajesGananciaAsync(restauranteId, platos, bebidas))
                .ReturnsAsync(porcentajesActualizados);

            var resultado = await CrearCasoDeUso().EjecutarAsync(restauranteId, platos, bebidas);

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Platos.Count + resultado.Bebidas.Count);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHayPorcentajeNegativoEnPlatos_LanzaArgumentException()
        {
            int restauranteId = 1;
            var platos = new List<DOM.PorcentajesCategoria>
            {
                new DOM.PorcentajesCategoria { Id = 1, Descripcion = "Entrada", Porcentaje = -5m }
            };
            var bebidas = new List<DOM.PorcentajesCategoria>
            {
                new DOM.PorcentajesCategoria { Id = 2, Descripcion = "Gaseosa", Porcentaje = 15m }
            };

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(restauranteId, platos, bebidas));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHayPorcentajeNegativoEnBebidas_LanzaArgumentException()
        {
            int restauranteId = 1;
            var platos = new List<DOM.PorcentajesCategoria>
            {
                new DOM.PorcentajesCategoria { Id = 1, Descripcion = "Entrada", Porcentaje = 10m }
            };
            var bebidas = new List<DOM.PorcentajesCategoria>
            {
                new DOM.PorcentajesCategoria { Id = 2, Descripcion = "Gaseosa", Porcentaje = -10m }
            };

            await Assert.ThrowsAsync<ArgumentException>(() => CrearCasoDeUso().EjecutarAsync(restauranteId, platos, bebidas));
        }
    }
}