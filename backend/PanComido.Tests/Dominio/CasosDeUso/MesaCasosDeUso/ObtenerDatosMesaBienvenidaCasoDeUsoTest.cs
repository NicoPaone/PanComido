using Moq;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using Microsoft.Extensions.Logging;

namespace PanComido.Tests.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class ObtenerDatosMesaBienvenidaCasoDeUsoTest
    {
        private readonly Mock<IRestauranteRepositorio> _restauranteMockRepo;
        private readonly Mock<IMesaRepositorio> _mesaMockRepo;

        public ObtenerDatosMesaBienvenidaCasoDeUsoTest()
        {
            _restauranteMockRepo = new Mock<IRestauranteRepositorio>();
            _mesaMockRepo = new Mock<IMesaRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTodoEsValido_DevuelveBienvenidaMesaDatos()
        {
            int restauranteId = 1;
            int mesaId = 1;

            var restauranteMock = new Restaurante { Id = restauranteId };
            var mesaMock = new MesaConPosiciones { Id = mesaId, EstadoMesa = EstadoMesa.Disponible };

            _restauranteMockRepo
                .Setup(r => r.ObtenerDatosDelLocalAsync(restauranteId))
                .ReturnsAsync(restauranteMock);

            _mesaMockRepo
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(mesaMock);

            var casoDeUso = new ObtenerDatosMesaBienvenidaCasoDeUso(_restauranteMockRepo.Object, _mesaMockRepo.Object, new Mock<ILogger<ObtenerDatosMesaBienvenidaCasoDeUso>>().Object);

            var resultado = await casoDeUso.EjecutarAsync(mesaId, restauranteId);

            Assert.NotNull(resultado);
            Assert.Equal(restauranteMock, resultado.RestauranteDatos);
            Assert.Equal(mesaMock, resultado.Mesa);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoRestauranteNoExiste_LanzaKeyNotFoundException()
        {
            int restauranteId = 1;
            int mesaId = 1;

            _restauranteMockRepo
                .Setup(r => r.ObtenerDatosDelLocalAsync(restauranteId))
                .ReturnsAsync((Restaurante)null);

            var casoDeUso = new ObtenerDatosMesaBienvenidaCasoDeUso(_restauranteMockRepo.Object, _mesaMockRepo.Object, new Mock<ILogger<ObtenerDatosMesaBienvenidaCasoDeUso>>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(mesaId, restauranteId));
        }

        [Fact]
        public async Task EjecutarAsync_CuandoMesaNoExiste_LanzaKeyNotFoundException()
        {
            int restauranteId = 1;
            int mesaId = 1;

            _restauranteMockRepo
                .Setup(r => r.ObtenerDatosDelLocalAsync(restauranteId))
                .ReturnsAsync(new Restaurante { Id = restauranteId });

            _mesaMockRepo
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync((MesaConPosiciones)null);

            var casoDeUso = new ObtenerDatosMesaBienvenidaCasoDeUso(_restauranteMockRepo.Object, _mesaMockRepo.Object, new Mock<ILogger<ObtenerDatosMesaBienvenidaCasoDeUso>>().Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => casoDeUso.EjecutarAsync(mesaId, restauranteId));
        }

        [Theory]
        [InlineData(EstadoMesa.Reservada)]
        [InlineData(EstadoMesa.Deshabilitada)]
        [InlineData(EstadoMesa.Ocupada)]
        public async Task EjecutarAsync_CuandoEstadoMesaInvalido_LanzaInvalidOperationException(EstadoMesa estado)
        {
            int restauranteId = 1;
            int mesaId = 1;

            _restauranteMockRepo
                .Setup(r => r.ObtenerDatosDelLocalAsync(restauranteId))
                .ReturnsAsync(new Restaurante { Id = restauranteId });

            _mesaMockRepo
                .Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId))
                .ReturnsAsync(new MesaConPosiciones { Id = mesaId, EstadoMesa = estado });

            var casoDeUso = new ObtenerDatosMesaBienvenidaCasoDeUso(_restauranteMockRepo.Object, _mesaMockRepo.Object, new Mock<ILogger<ObtenerDatosMesaBienvenidaCasoDeUso>>().Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => casoDeUso.EjecutarAsync(mesaId, restauranteId));
        }
    }
}
