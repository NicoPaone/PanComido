using Moq;
using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Servicios;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Insumos
{
    public class ObtenerInsumoPorIdCasoDeUsoTest
    {
        private readonly Mock<IInsumoRepositorio> _insumoRepoMock;
        private readonly ObtenerInsumoPorIdCasoDeUso _casoDeUso;

        public ObtenerInsumoPorIdCasoDeUsoTest()
        {
            _insumoRepoMock = new Mock<IInsumoRepositorio>();
            _casoDeUso = new ObtenerInsumoPorIdCasoDeUso(_insumoRepoMock.Object, new UltimoPrecioCompraInsumoServicio());
        }

        [Fact]
        public async Task EjecutarAsync_CuandoInsumoExiste_RetornaInsumo()
        {
            int insumoId = 5;
            int restauranteId = 1;
            Insumo insumoEsperado = new Insumo { Id = insumoId, Nombre = "Harina" };

            _insumoRepoMock.Setup(r => r.ObtenerPorIdAsync(insumoId, restauranteId))
                .ReturnsAsync(insumoEsperado);

            Insumo resultado = await _casoDeUso.EjecutarAsync(insumoId, restauranteId);

            Assert.NotNull(resultado);
            Assert.Equal(insumoId, resultado.Id);
            Assert.Equal("Harina", resultado.Nombre);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoInsumoNoExiste_LanzaKeyNotFoundException()
        {
            int insumoId = 99;
            int restauranteId = 1;

            _insumoRepoMock.Setup(r => r.ObtenerPorIdAsync(insumoId, restauranteId))
                .ReturnsAsync((Insumo)null);

            KeyNotFoundException excepcion = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _casoDeUso.EjecutarAsync(insumoId, restauranteId));

            Assert.Equal("El insumo no existe o no pertenece al restaurante.", excepcion.Message);
        }
    }
}
