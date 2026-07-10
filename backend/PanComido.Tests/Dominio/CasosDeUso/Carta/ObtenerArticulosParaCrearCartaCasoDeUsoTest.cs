using Moq;
using PanComido.Dominio.CasosDeUso.CartaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Dominio.Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Carta
{
    public class ObtenerArticulosParaCrearCartaCasoDeUsoTest
    {
        private readonly Mock<IArticuloRepositorio> _articuloRepoMock;
        private readonly Mock<ITiempoDePreparacionPlatoServicio> _tiempoPreparacionServicioMock;
        private readonly IUltimoPrecioCompraInsumoServicio _ultimoPrecioCompraServicio;
        private readonly ObtenerArticulosParaCrearCartaCasoDeUso _casoDeUso;

        public ObtenerArticulosParaCrearCartaCasoDeUsoTest()
        {
            _articuloRepoMock = new Mock<IArticuloRepositorio>();
            _tiempoPreparacionServicioMock = new Mock<ITiempoDePreparacionPlatoServicio>();
            _ultimoPrecioCompraServicio = new UltimoPrecioCompraInsumoServicio();
            _casoDeUso = new ObtenerArticulosParaCrearCartaCasoDeUso(_articuloRepoMock.Object, _tiempoPreparacionServicioMock.Object, _ultimoPrecioCompraServicio);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoArticuloEsBebida_CalculaCostoDelUltimoPedido()
        {
            // Preparar
            var bebida = new Insumo
            {
                Id = 1,
                PedidoInsumos = new List<PedidoInsumo>
                {
                    new PedidoInsumo { PrecioCompra = 1500, Estado = EstadoPedido.Recibido, Fecha = new DateOnly(2026, 7, 1) }
                }
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
            var tomate = new Insumo { PedidoInsumos = new List<PedidoInsumo> { new PedidoInsumo { PrecioCompra = 100, Estado = EstadoPedido.Recibido, Fecha = new DateOnly(2026, 7, 1) } } };
            var carne = new Insumo { PedidoInsumos = new List<PedidoInsumo> { new PedidoInsumo { PrecioCompra = 1000, Estado = EstadoPedido.Recibido, Fecha = new DateOnly(2026, 7, 1) } } };

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

        [Fact]
        public async Task EjecutarAsync_CuandoHayPedidosPendientesYRecibidos_IgnoraLosPendientesYUsaElRecibidoMasReciente()
        {
            // Preparar: 3 pedidos históricos de la misma bebida.
            // El más reciente de todos es Pendiente (todavía no llegó) y no debe contarse.
            var bebida = new Insumo
            {
                Id = 1,
                PedidoInsumos = new List<PedidoInsumo>
                {
                    new PedidoInsumo { PrecioCompra = 100, Estado = EstadoPedido.Recibido, Fecha = new DateOnly(2026, 1, 1) },
                    new PedidoInsumo { PrecioCompra = 150, Estado = EstadoPedido.Recibido, Fecha = new DateOnly(2026, 6, 1) },
                    new PedidoInsumo { PrecioCompra = 999, Estado = EstadoPedido.Pendiente, Fecha = new DateOnly(2026, 7, 5) }
                }
            };

            _articuloRepoMock.Setup(r => r.ObtenerTodosLosArticulosParaCartaAsync(1))
                             .ReturnsAsync(new List<Articulo> { bebida });

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(1);

            // Verificar: toma el Recibido más reciente (150), no el Pendiente (999) aunque sea más nuevo.
            Assert.Single(resultado);
            Assert.Equal(150, resultado[0].CostoCalculado);
        }
    }
}
