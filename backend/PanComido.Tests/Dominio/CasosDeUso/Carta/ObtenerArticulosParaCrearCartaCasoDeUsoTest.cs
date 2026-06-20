using Moq;
using PanComido.Dominio.CasosDeUso.CartaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Carta
{
    public class ObtenerArticulosParaCrearCartaCasoDeUsoTest
    {
        private readonly Mock<IArticuloRepositorio> _articuloRepoMock;
        private readonly Mock<ITiempoDePreparacionPlatoServicio> _tiempoPreparacionServicioMock;
        private readonly ObtenerArticulosParaCrearCartaCasoDeUso _casoDeUso;

        public ObtenerArticulosParaCrearCartaCasoDeUsoTest()
        {
            _articuloRepoMock = new Mock<IArticuloRepositorio>();
            _tiempoPreparacionServicioMock = new Mock<ITiempoDePreparacionPlatoServicio>();
            _casoDeUso = new ObtenerArticulosParaCrearCartaCasoDeUso(_articuloRepoMock.Object, _tiempoPreparacionServicioMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoArticuloEsBebida_CalculaCostoDelUltimoPedido()
        {
            // Preparar
            var bebida = new Insumo 
            { 
                Id = 1,
                PedidoInsumos = new List<PedidoInsumo> { new PedidoInsumo { PrecioCompra = 1500 } }
            };

            _articuloRepoMock.Setup(r => r.ObtenerTodosLosArticulosParaCartaAsync(1))
                             .ReturnsAsync(new List<Articulo> { bebida });

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(1);

            // Verificar
            Assert.Single(resultado);
            Assert.Equal(1500, resultado[0].CostoCalculado);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoArticuloEsPlato_CalculaCostoSumandoSusIngredientes()
        {
            // Preparar
            // Tomate: $100 el kg. Carne: $1000 el kg.
            var tomate = new Insumo { PedidoInsumos = new List<PedidoInsumo> { new PedidoInsumo { PrecioCompra = 100 } } }; 
            var carne = new Insumo { PedidoInsumos = new List<PedidoInsumo> { new PedidoInsumo { PrecioCompra = 1000 } } }; 

            var plato = new Plato 
            { 
                Id = 2,
                Ingredientes = new List<PlatoIngrediente>
                {
                    new PlatoIngrediente { Insumo = tomate, Cantidad = 0.5M }, // 0.5 * 100 = $50
                    new PlatoIngrediente { Insumo = carne, Cantidad = 0.2M }   // 0.2 * 1000 = $200
                }
            };

            _articuloRepoMock.Setup(r => r.ObtenerTodosLosArticulosParaCartaAsync(1))
                             .ReturnsAsync(new List<Articulo> { plato });

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(1);

            // Verificar
            Assert.Single(resultado);
            Assert.Equal(250, resultado[0].CostoCalculado); // 50 + 200 = 250
        }
    }
}
