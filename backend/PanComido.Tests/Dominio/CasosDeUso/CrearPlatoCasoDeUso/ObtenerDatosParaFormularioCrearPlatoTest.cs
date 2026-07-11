using Moq;
using PanComido.Dominio.CasosDeUso.CrearPlatoCasoDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.CrearPlatoCasoDeUso
{
    public class ObtenerDatosParaFormularioCrearPlatoTest
    {
        private readonly Mock<IFormularioPlatoRepositorio> _mockFormularioPlatoRepositorio;
        private readonly Mock<IPorcentajesCategoriaRepositorio> _mockPorcentajesRepositorio;
        private readonly Mock<IUltimoPrecioCompraInsumoServicio> _mockUltimoPrecioCompraServicio;
        private readonly ObtenerDatosParaFormularioCrearPlato _casoDeUso;

        public ObtenerDatosParaFormularioCrearPlatoTest()
        {
            _mockFormularioPlatoRepositorio = new Mock<IFormularioPlatoRepositorio>();
            _mockPorcentajesRepositorio = new Mock<IPorcentajesCategoriaRepositorio>();
            _mockUltimoPrecioCompraServicio = new Mock<IUltimoPrecioCompraInsumoServicio>();
            
            _casoDeUso = new ObtenerDatosParaFormularioCrearPlato(
                _mockFormularioPlatoRepositorio.Object,
                _mockPorcentajesRepositorio.Object,
                _mockUltimoPrecioCompraServicio.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarDatosFormulario()
        {
            // Preparar
            int restauranteId = 1;

            var ingredientes = new List<Ingrediente>
            {
                new Ingrediente { Id = 1, Nombre = "Tomate", PedidoInsumos = new List<PedidoInsumo>() }
            };

            _mockFormularioPlatoRepositorio.Setup(r => r.ObtenerIngredientesBaseAsync(restauranteId))
                .ReturnsAsync(ingredientes);
                
            _mockFormularioPlatoRepositorio.Setup(r => r.ObtenerTiposPlatoAsync())
                .ReturnsAsync(new List<TipoPlato>());
                
            _mockFormularioPlatoRepositorio.Setup(r => r.ObtenerCategoriasPlatoAsync())
                .ReturnsAsync(new List<CategoriaPlato>());
                
            _mockFormularioPlatoRepositorio.Setup(r => r.ObtenerRestriccionesAsync())
                .ReturnsAsync(new List<Restriccion>());
                
            _mockFormularioPlatoRepositorio.Setup(r => r.ObtenerIngredientesPreparadosAsync(restauranteId))
                .ReturnsAsync(new List<IngredientePreparado>());

            _mockPorcentajesRepositorio.Setup(r => r.ObtenerPorcentajesGananciaAsync(restauranteId))
                .ReturnsAsync(new PorcentajesGanancia());

            _mockUltimoPrecioCompraServicio.Setup(s => s.ObtenerUltimoPrecioCompraRecibido(It.IsAny<List<PedidoInsumo>>()))
                .Returns(15.5m);

            // Actuar
            var resultado = await _casoDeUso.Ejecutar(restauranteId);

            // Verificar
            Assert.NotNull(resultado);
            Assert.NotNull(resultado.TiposPlato);
            Assert.NotNull(resultado.CategoriasPlato);
            Assert.NotNull(resultado.Restricciones);
            Assert.NotNull(resultado.Ingredientes);
            Assert.NotNull(resultado.IngredientePreparados);
            Assert.NotNull(resultado.Porcentajes);

            Assert.Equal(15.5m, resultado.Ingredientes[0].CostoUnitario);

            _mockFormularioPlatoRepositorio.Verify(r => r.ObtenerIngredientesBaseAsync(restauranteId), Times.Once);
            _mockUltimoPrecioCompraServicio.Verify(s => s.ObtenerUltimoPrecioCompraRecibido(It.IsAny<List<PedidoInsumo>>()), Times.Once);
        }
    }
}
