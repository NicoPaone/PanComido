using Moq;
using PanComido.Dominio.CasosDeUso.MiseAndPlaceCasoDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.MiseAndPlace
{
    public class ObtenerIngredientesParaCrearMiseAndPlaceTest
    {
        private readonly Mock<IFormularioPlatoRepositorio> _mockFormularioRepositorio;
        private readonly Mock<ICategoriaInsumoRepositorio> _mockCategoriaRepositorio;
        private readonly Mock<IUnidadMedidaRepositorio> _mockUnidadMedidaRepositorio;
        private readonly Mock<IBodegaRepositorio> _mockBodegaRepositorio;
        private readonly Mock<IUltimoPrecioCompraInsumoServicio> _mockPrecioServicio;
        private readonly ObtenerIngredientesParaCrearMiseAndPlace _casoDeUso;

        public ObtenerIngredientesParaCrearMiseAndPlaceTest()
        {
            _mockFormularioRepositorio = new Mock<IFormularioPlatoRepositorio>();
            _mockCategoriaRepositorio = new Mock<ICategoriaInsumoRepositorio>();
            _mockUnidadMedidaRepositorio = new Mock<IUnidadMedidaRepositorio>();
            _mockBodegaRepositorio = new Mock<IBodegaRepositorio>();
            _mockPrecioServicio = new Mock<IUltimoPrecioCompraInsumoServicio>();

            _casoDeUso = new ObtenerIngredientesParaCrearMiseAndPlace(
                _mockFormularioRepositorio.Object,
                _mockCategoriaRepositorio.Object,
                _mockUnidadMedidaRepositorio.Object,
                _mockBodegaRepositorio.Object,
                _mockPrecioServicio.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CaminoFeliz_RetornaDatosYCostoUnitario()
        {
            // Preparar
            int restauranteId = 1;
            
            var pedidos = new List<PedidoInsumo> { new PedidoInsumo { InsumoId = 1, PrecioCompra = 150m } };
            var ingrediente = new Ingrediente { Id = 1, Nombre = "Harina", PedidoInsumos = pedidos };
            var ingredientes = new List<Ingrediente> { ingrediente };
            
            var categorias = new List<CategoriaInsumo> 
            { 
                new CategoriaInsumo { Id = 1, TipoAplica = TipoInsumo.Ingrediente },
                new CategoriaInsumo { Id = 2, TipoAplica = TipoInsumo.Bebida } // Debería filtrarse
            };
            
            var unidades = new List<UnidadMedida> { new UnidadMedida { Id = 1, Nombre = "Kilo" } };
            var bodegas = new List<Bodega> { new Bodega { Id = 1, Nombre = "Principal" } };

            _mockFormularioRepositorio.Setup(x => x.ObtenerIngredientesBaseAsync(restauranteId))
                .ReturnsAsync(ingredientes);
                
            _mockCategoriaRepositorio.Setup(x => x.ObtenerCategoriasInsumoAsync())
                .ReturnsAsync(categorias);
                
            _mockUnidadMedidaRepositorio.Setup(x => x.ObtenerUnidadesDeMedidaAsync())
                .ReturnsAsync(unidades);
                
            _mockBodegaRepositorio.Setup(x => x.ObtenerBodegasAsync(restauranteId))
                .ReturnsAsync(bodegas);

            _mockPrecioServicio.Setup(x => x.ObtenerUltimoPrecioCompraRecibido(pedidos))
                .Returns(150m);

            // Actuar
            var (resIngredientes, resCategorias, resUnidades, resBodegas) = await _casoDeUso.EjecutarAsync(restauranteId);

            // Verificar
            Assert.Single(resIngredientes);
            Assert.Equal(150m, resIngredientes[0].CostoUnitario); // Verificamos que se seteó el costo unitario

            Assert.Single(resCategorias); // Verificamos que se filtró la Bebida
            Assert.Equal(TipoInsumo.Ingrediente, resCategorias[0].TipoAplica);

            Assert.Single(resUnidades);
            Assert.Single(resBodegas);
        }
    }
}
