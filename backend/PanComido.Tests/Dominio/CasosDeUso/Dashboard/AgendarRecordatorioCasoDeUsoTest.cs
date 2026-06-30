using Moq;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.IA;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Repositorios.IA;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Dashboard
{
    public class AgendarRecordatorioCasoDeUsoTest
    {
        private readonly Mock<IArticuloRepositorio> _articuloRepoMock;
        private readonly Mock<ISugerenciaIARepositorio> _sugerenciaIaRepoMock;
        private readonly Mock<IPlatoAnalisisRepositorio> _platoAnalisisRepoMock;
        private readonly AgendarRecordatorioCasoDeUso _casoDeUso;

        public AgendarRecordatorioCasoDeUsoTest()
        {
            _articuloRepoMock = new Mock<IArticuloRepositorio>();
            _sugerenciaIaRepoMock = new Mock<ISugerenciaIARepositorio>();
            _platoAnalisisRepoMock = new Mock<IPlatoAnalisisRepositorio>();
            _casoDeUso = new AgendarRecordatorioCasoDeUso(
                _articuloRepoMock.Object,
                _sugerenciaIaRepoMock.Object,
                _platoAnalisisRepoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_DebeGuardarNotificacionYMarcarSugerenciaComoAplicada()
        {
            int restauranteId = 1;
            int platoId = 10;
            string accionSugerida = "Combo de Papas con Gaseosa";
            var plato = new Plato
            {
                Id = platoId,
                Nombre = "Papas Fritas"
            };

            _articuloRepoMock.Setup(r => r.ObtenerDetalleAsync(restauranteId, platoId))
                .ReturnsAsync(plato);

            var sugerenciaIa = new SugerenciaIA
            {
                PlatosAnalisis = new List<PlatoAnalisisIa>
                {
                    new PlatoAnalisisIa
                    {
                        PlatoId = platoId,
                        Sugerencias = new List<PlatoSugerenciaIa>
                        {
                            new PlatoSugerenciaIa { Id = 2, Tipo = "combo", Accion = "Combo de Papas con Gaseosa", Aplicada = false }
                        }
                    }
                }
            };

            _sugerenciaIaRepoMock.Setup(r => r.ObtenerSugerenciaIAAsync(restauranteId))
                .ReturnsAsync(sugerenciaIa);

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, platoId, accionSugerida);

            Assert.NotNull(resultado);
            Assert.Equal("Recordatorio guardado en el módulo de tareas administrativas.", resultado.Mensaje);
            Assert.Equal("Revisión: Papas Fritas", resultado.Titulo);
            Assert.True(sugerenciaIa.PlatosAnalisis[0].Sugerencias[0].Aplicada);

            _platoAnalisisRepoMock.Verify(r => r.GuardarRecordatorioNotificacionAsync(restauranteId, It.Is<string>(s => s.Contains("Revisión: Papas Fritas") && s.Contains("Combo de Papas con Gaseosa"))), Times.Once);
            _sugerenciaIaRepoMock.Verify(r => r.GuardarSugerenciaIAAsync(restauranteId, sugerenciaIa), Times.Once);
        }
    }
}
