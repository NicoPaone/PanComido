using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Dashboard;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Repositorios.IA;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Dominio.Interfaces.Servicios.IA;
using PanComido.Presentacion.Controllers;
using PanComido.Presentacion.DTOs.Dashboard;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Mappers.Dashboard;

namespace PanComido.Tests.Presentacion.Controllers
{
    public class DashboardPlatoAnalisisControllerTest
    {
        private const int RestauranteId = 7;

        [Fact]
        public async Task Obtener_CuandoPlatoNoExiste_RetornaNotFound()
        {
            var platoAnalisisRepo = new Mock<IPlatoAnalisisRepositorio>();
            platoAnalisisRepo
                .Setup(r => r.ObtenerArticuloConPlatoYIngredientesPorNombreAsync(RestauranteId, "Pizza"))
                .ReturnsAsync((Articulo?)null);

            var controller = CrearController(platoAnalisisRepo: platoAnalisisRepo);

            var resultado = await controller.Obtener("Pizza");

            var notFound = Assert.IsType<NotFoundObjectResult>(resultado.Result);
            var error = Assert.IsType<ErrorResponseDto>(notFound.Value);
            Assert.Equal("not_found", error.Code);
            Assert.Equal("No se encontró el plato especificado.", error.Error);
        }

        [Fact]
        public async Task AplicarDescuento_CuandoDescuentoEsInvalido_RetornaBadRequest()
        {
            var politica = new Mock<IPoliticaDescuentoDashboardServicio>();
            politica
                .Setup(p => p.ObtenerAsync(RestauranteId))
                .ReturnsAsync(new PoliticaDescuentoDashboard
                {
                    PorcentajeDescuentoMaximo = 80m,
                    MargenMinimoPermitido = 20m
                });

            var controller = CrearController(politica: politica);

            var resultado = await controller.AplicarDescuento(new AplicarDescuentoRequest
            {
                PlatoId = 1,
                PorcentajeDescuento = 0
            });

            var badRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            var error = Assert.IsType<ErrorResponseDto>(badRequest.Value);
            Assert.Equal("bad_request", error.Code);
            Assert.Contains("descuento debe ser mayor", error.Error);
        }

        private static DashboardPlatoAnalisisController CrearController(
            Mock<IPlatoAnalisisRepositorio>? platoAnalisisRepo = null,
            Mock<IPoliticaDescuentoDashboardServicio>? politica = null)
        {
            platoAnalisisRepo ??= new Mock<IPlatoAnalisisRepositorio>();
            politica ??= new Mock<IPoliticaDescuentoDashboardServicio>();

            var sugerenciaRepo = new Mock<ISugerenciaIARepositorio>();
            var calculadorCosto = new Mock<ICalculadorCostoPlatoServicio>();
            var dateTimeProvider = new Mock<IDateTimeProvider>();
            var sugerenciaIaServicio = new Mock<ISugerenciaPlatosIAServicio>();
            var articuloRepo = new Mock<IArticuloRepositorio>();
            var transaccion = new Mock<ITransaccionPersistenciaServicio>();

            var controller = new DashboardPlatoAnalisisController(
                new ObtenerAnalisisPlatoCasoDeUso(
                    platoAnalisisRepo.Object,
                    sugerenciaRepo.Object,
                    calculadorCosto.Object,
                    dateTimeProvider.Object,
                    sugerenciaIaServicio.Object,
                    NullLogger<ObtenerAnalisisPlatoCasoDeUso>.Instance),
                new AplicarDescuentoCasoDeUso(
                    articuloRepo.Object,
                    sugerenciaRepo.Object,
                    calculadorCosto.Object,
                    politica.Object,
                    transaccion.Object),
                new AgendarRecordatorioCasoDeUso(
                    articuloRepo.Object,
                    sugerenciaRepo.Object,
                    platoAnalisisRepo.Object),
                new PlatoAnalisisMapper());

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.HttpContext.Items["restauranteId"] = RestauranteId;

            return controller;
        }
    }
}
