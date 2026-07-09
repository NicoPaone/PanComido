using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PanComido.Dominio.CasosDeUso.EmpleadoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Presentacion.Controllers;
using PanComido.Presentacion.DTOs.Empleado;
using PanComido.Presentacion.DTOs.ErrorResponse;
using PanComido.Presentacion.Mappers;

namespace PanComido.Tests.Presentacion.Controllers
{
    public class EmpleadoControllerTest
    {
        private const int RestauranteId = 7;

        [Fact]
        public async Task Crear_CuandoEmailYaExiste_RetornaBadRequest()
        {
            var repo = new Mock<IEmpleadoRepositorio>();
            repo.Setup(r => r.ObtenerPorEmailAsync("lucia@pancomido.com"))
                .ReturnsAsync(new Empleado { Id = 10, Email = "lucia@pancomido.com" });

            var controller = CrearController(repo);

            var resultado = await controller.Crear(new CrearEmpleadoRequestDto
            {
                Nombre = "Lucia",
                Email = "lucia@pancomido.com",
                Contrasenia = "password123",
                Estado = "Activo",
                Rol = "Mozo",
                TurnosIds = new List<int>()
            });

            var badRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            var error = Assert.IsType<ErrorResponseDto>(badRequest.Value);
            Assert.Equal("bad_request", error.Code);
            Assert.Contains("ya se encuentra registrado", error.Error);
        }

        [Fact]
        public async Task Modificar_CuandoEmpleadoNoExiste_RetornaNotFound()
        {
            var repo = new Mock<IEmpleadoRepositorio>();
            repo.Setup(r => r.ObtenerPorIdYRestauranteAsync(99, RestauranteId))
                .ReturnsAsync((Empleado?)null);

            var controller = CrearController(repo);

            var resultado = await controller.Modificar(99, new ModificarEmpleadoRequestDto
            {
                Nombre = "Lucia",
                Email = "lucia@pancomido.com",
                Estado = "Activo",
                Rol = "Mozo",
                TurnosIds = new List<int>()
            });

            var notFound = Assert.IsType<NotFoundObjectResult>(resultado.Result);
            var error = Assert.IsType<ErrorResponseDto>(notFound.Value);
            Assert.Equal("not_found", error.Code);
            Assert.Equal("Empleado no encontrado.", error.Error);
        }

        [Fact]
        public async Task Eliminar_CuandoEmpleadoNoExiste_RetornaNotFound()
        {
            var repo = new Mock<IEmpleadoRepositorio>();
            repo.Setup(r => r.ObtenerPorIdYRestauranteAsync(99, RestauranteId))
                .ReturnsAsync((Empleado?)null);

            var controller = CrearController(repo);

            var resultado = await controller.Eliminar(99);

            var notFound = Assert.IsType<NotFoundObjectResult>(resultado.Result);
            var error = Assert.IsType<ErrorResponseDto>(notFound.Value);
            Assert.Equal("not_found", error.Code);
            Assert.Equal("Empleado no encontrado.", error.Error);
        }

        private static EmpleadoController CrearController(Mock<IEmpleadoRepositorio> repo)
        {
            var hasher = new Mock<IContraseniaHasher>();
            hasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hash");

            var mapper = new EmpleadoMapper(new TurnoLaboralMapper());
            var controller = new EmpleadoController(
                new ListarEmpleadosCasoDeUso(repo.Object),
                new CrearEmpleadoCasoDeUso(repo.Object, hasher.Object),
                new ModificarEmpleadoCasoDeUso(repo.Object, hasher.Object),
                new EliminarEmpleadoCasoDeUso(repo.Object),
                mapper);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.HttpContext.Items["restauranteId"] = RestauranteId;

            return controller;
        }
    }
}
