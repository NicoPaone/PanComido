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
    public class ObtenerResumenOperativoCasoDeUsoTest
    {
        private readonly Mock<IComandaRepositorio> _comandaRepoMock;
        private readonly Mock<IPlatoAnalisisRepositorio> _platoAnalisisRepoMock;
        private readonly ObtenerResumenOperativoCasoDeUso _casoDeUso;

        public ObtenerResumenOperativoCasoDeUsoTest()
        {
            _comandaRepoMock = new Mock<IComandaRepositorio>();
            _platoAnalisisRepoMock = new Mock<IPlatoAnalisisRepositorio>();
            _platoAnalisisRepoMock.Setup(r => r.ObtenerRecordatoriosActivosAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<Notificacion>());

            _casoDeUso = new ObtenerResumenOperativoCasoDeUso(_comandaRepoMock.Object, _platoAnalisisRepoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CalculaPorcentajesYVariacionesCorrectamente()
        {
            // Preparar
            int restauranteId = 1;
            var desde = new DateTime(2023, 1, 1);
            var hasta = new DateTime(2023, 1, 10);
            
            var totalesActuales = new TotalesPeriodo { TotalFacturado = 10000, CantidadPedidos = 100 };
            var totalesAnteriores = new TotalesPeriodo { TotalFacturado = 5000, CantidadPedidos = 50 };
            var ventasAgrupadas = new List<VentaAgrupada> { new VentaAgrupada { Etiqueta = "Dia 1", Total = 1000 } };

            _comandaRepoMock.Setup(r => r.ObtenerTotalesPeriodoAsync(
                    restauranteId, 
                    desde, 
                    It.IsAny<DateTime>()))
                .ReturnsAsync(totalesActuales);

            _comandaRepoMock.Setup(r => r.ObtenerTotalesPeriodoAsync(
                    restauranteId, 
                    It.Is<DateTime>(d => d < desde), 
                    It.Is<DateTime>(d => d < hasta)))
                .ReturnsAsync(totalesAnteriores);

            _comandaRepoMock.Setup(r => r.ObtenerVentasAgrupadasAsync(
                    restauranteId, 
                    desde, 
                    It.IsAny<DateTime>(), 
                    It.IsAny<string>()))
                .ReturnsAsync(ventasAgrupadas);

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, desde, hasta);

            // Verificar
            Assert.NotNull(resultado);
            Assert.Equal(10000, resultado.TotalVentas);
            Assert.Equal(100, resultado.TotalPedidos);
            Assert.Equal(100, resultado.TicketPromedio); 
            Assert.Equal(10, resultado.PromedioDiarioPedidos);
            
            // 10000 es 100% mas que 5000
            Assert.Equal(100m, resultado.VariacionVentas); 
            // 100 pedidos es 100% mas que 50 pedidos
            Assert.Equal(100m, resultado.VariacionPedidos); 
            // El ticket es el mismo (100 vs 100), variacion = 0
            Assert.Equal(0m, resultado.VariacionTicket); 
            Assert.Equal(ventasAgrupadas, resultado.Grafico);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoHayVentasAnteriores_VariacionEsCienPorCiento()
        {
            // Preparar
            int restauranteId = 1;
            var desde = new DateTime(2023, 1, 1);
            var hasta = new DateTime(2023, 1, 10);
            
            var totalesActuales = new TotalesPeriodo { TotalFacturado = 10000, CantidadPedidos = 100 };
            var totalesAnteriores = new TotalesPeriodo { TotalFacturado = 0, CantidadPedidos = 0 };

            _comandaRepoMock.SetupSequence(r => r.ObtenerTotalesPeriodoAsync(restauranteId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(totalesActuales)
                .ReturnsAsync(totalesAnteriores);

            _comandaRepoMock.Setup(r => r.ObtenerVentasAgrupadasAsync(restauranteId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync(new List<VentaAgrupada>());

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, desde, hasta);

            // Verificar
            Assert.Equal(100m, resultado.VariacionVentas); 
            Assert.Equal(100m, resultado.VariacionPedidos); 
            Assert.Equal(100m, resultado.VariacionTicket); 
        }
    }
}
