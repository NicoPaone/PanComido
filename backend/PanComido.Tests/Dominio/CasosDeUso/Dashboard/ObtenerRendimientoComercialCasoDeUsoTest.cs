using Moq;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Dashboard
{
    public class ObtenerRendimientoComercialCasoDeUsoTest
    {
        private readonly Mock<IPlatoAnalisisRepositorio> _platoAnalisisRepoMock;
        private readonly ObtenerRendimientoComercialCasoDeUso _casoDeUso;

        public ObtenerRendimientoComercialCasoDeUsoTest()
        {
            _platoAnalisisRepoMock = new Mock<IPlatoAnalisisRepositorio>();
            _casoDeUso = new ObtenerRendimientoComercialCasoDeUso(_platoAnalisisRepoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_DebeLlamarAlRepositorioYRetornarResumenRendimiento()
        {
            int restauranteId = 1;
            var desde = new DateTime(2023, 1, 1);
            var hasta = new DateTime(2023, 1, 31);
            
            var platosMasVendidos = new List<RendimientoPlato> { new RendimientoPlato { PlatoId = 1, UnidadesVendidas = 10 } };
            var platosMenosVendidos = new List<RendimientoPlato> { new RendimientoPlato { PlatoId = 2, UnidadesVendidas = 2 } };

            _platoAnalisisRepoMock.Setup(r => r.ObtenerTopPlatosMasVendidosAsync(
                    restauranteId, 
                    It.IsAny<DateTime>(), 
                    It.IsAny<DateTime>(), 
                    5))
                .ReturnsAsync(platosMasVendidos);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerTopPlatosMenosVendidosAsync(
                    restauranteId, 
                    It.IsAny<DateTime>(), 
                    It.IsAny<DateTime>(), 
                    5))
                .ReturnsAsync(platosMenosVendidos);

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, desde, hasta);

            Assert.NotNull(resultado);
            Assert.Equal(platosMasVendidos, resultado.MasVendidos);
            Assert.Equal(platosMenosVendidos, resultado.MenosVendidos);

            _platoAnalisisRepoMock.Verify(r => r.ObtenerTopPlatosMasVendidosAsync(restauranteId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), 5), Times.Once);
            _platoAnalisisRepoMock.Verify(r => r.ObtenerTopPlatosMenosVendidosAsync(restauranteId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), 5), Times.Once);
        }
    }
}
