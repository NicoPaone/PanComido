using Moq;
using PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.CasosDeUso.Proveedor
{
    public class ObtenerInsumosParaPedidoCasoDeUsoTest
    {
        private readonly Mock<IInsumoRepositorio> _insumoRepoMock;
        private readonly Mock<ILoteRepositorio> _loteRepoMock;
        private readonly Mock<IEstadoStockInsumoServicio> _estadoInsumoServicioMock;
        private readonly Mock<IPedidoRepositorio> _pedidoRepoMock;

        public ObtenerInsumosParaPedidoCasoDeUsoTest()
        {
            _insumoRepoMock = new Mock<IInsumoRepositorio>();
            _loteRepoMock = new Mock<ILoteRepositorio>();
            _estadoInsumoServicioMock = new Mock<IEstadoStockInsumoServicio>();
            _pedidoRepoMock = new Mock<IPedidoRepositorio>();
        }

        [Fact]
        public async Task EjecutarAsync_CuandoInsumoEstaCritico_DevuelveConSugerenciaDobleDelMinimo()
        {
            int proveedorId = 1;
            int restauranteId = 1;

            var insumosProveedor = new List<DOM.Insumo>
            {
                new DOM.Insumo { Id = 10, Nombre = "Tomate"},

            };

            var insumosResto = new List<DOM.Insumo>
            {
                new DOM.Insumo { Id = 10, Nombre = "Tomate", StockMinimo = 10},
            };

            _insumoRepoMock
                .Setup(r => r.ObtenerInsumosDelProveedorAsync(proveedorId, restauranteId))
                .ReturnsAsync(insumosProveedor);

            _insumoRepoMock
                .Setup(r => r.ObtenerInsumosAsync(restauranteId))
                .ReturnsAsync(insumosResto);

            _loteRepoMock
               .Setup(r => r.ObtenerStockTotalDeInsumo(insumosResto[0].Id))
               .ReturnsAsync(2);

            _estadoInsumoServicioMock
                .Setup(r => r.CalcularEstadoStock(2, 10))
                .Returns(EstadoStock.Critico);

            _pedidoRepoMock
                .Setup(r => r.ObtenerInsumosEnPedidosNoRecibidosAsync(proveedorId))
                .ReturnsAsync(new List<int>());

            var casoDeUso = new ObtenerInsumosParaPedidoCasoDeUso(
                _insumoRepoMock.Object,
                _estadoInsumoServicioMock.Object,
                _loteRepoMock.Object,
                _pedidoRepoMock.Object
                );

            var resultado = await casoDeUso.EjecutarAsync(proveedorId, restauranteId);

            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal(20, resultado[0].CantidadSugerida);
            Assert.Equal("Critico", resultado[0].EstadoStock);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoInsumoEstaBajo_DevuelveConSugerenciaIgualAlMinimo()
        {
            int proveedorId = 1;
            int restauranteId = 1;

            var insumosProveedor = new List<DOM.Insumo>
            {
                new DOM.Insumo { Id = 10, Nombre = "Tomate"},

            };

            var insumosResto = new List<DOM.Insumo>
            {
                new DOM.Insumo { Id = 10, Nombre = "Tomate", StockMinimo = 10},
            };

            _insumoRepoMock
                .Setup(r => r.ObtenerInsumosDelProveedorAsync(proveedorId, restauranteId))
                .ReturnsAsync(insumosProveedor);

            _insumoRepoMock
                .Setup(r => r.ObtenerInsumosAsync(restauranteId))
                .ReturnsAsync(insumosResto);

            _loteRepoMock
               .Setup(r => r.ObtenerStockTotalDeInsumo(insumosResto[0].Id))
               .ReturnsAsync(8);

            _estadoInsumoServicioMock
                .Setup(r => r.CalcularEstadoStock(8, 10))
                .Returns(EstadoStock.Bajo);

            _pedidoRepoMock
                .Setup(r => r.ObtenerInsumosEnPedidosNoRecibidosAsync(proveedorId))
                .ReturnsAsync(new List<int>());

            var casoDeUso = new ObtenerInsumosParaPedidoCasoDeUso(
                _insumoRepoMock.Object,
                _estadoInsumoServicioMock.Object,
                _loteRepoMock.Object,
                _pedidoRepoMock.Object
                );

            var resultado = await casoDeUso.EjecutarAsync(proveedorId, restauranteId);

            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal(10, resultado[0].CantidadSugerida);
            Assert.Equal("Bajo", resultado[0].EstadoStock);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoInsumoEstaNormal_NoLoDevuelve()
        {
            int proveedorId = 1;
            int restauranteId = 1;

            var insumosProveedor = new List<DOM.Insumo>
            {
                new DOM.Insumo { Id = 10, Nombre = "Tomate"},

            };

            var insumosResto = new List<DOM.Insumo>
            {
                new DOM.Insumo { Id = 10, Nombre = "Tomate", StockMinimo = 10},
            };

            _insumoRepoMock
                .Setup(r => r.ObtenerInsumosDelProveedorAsync(proveedorId, restauranteId))
                .ReturnsAsync(insumosProveedor);

            _insumoRepoMock
                .Setup(r => r.ObtenerInsumosAsync(restauranteId))
                .ReturnsAsync(insumosResto);

            _loteRepoMock
               .Setup(r => r.ObtenerStockTotalDeInsumo(insumosResto[0].Id))
               .ReturnsAsync(30);

            _estadoInsumoServicioMock
                .Setup(r => r.CalcularEstadoStock(30, 10))
                .Returns(EstadoStock.Normal);

            _pedidoRepoMock
                .Setup(r => r.ObtenerInsumosEnPedidosNoRecibidosAsync(proveedorId))
                .ReturnsAsync(new List<int>());

            var casoDeUso = new ObtenerInsumosParaPedidoCasoDeUso(
                _insumoRepoMock.Object,
                _estadoInsumoServicioMock.Object,
                _loteRepoMock.Object,
                _pedidoRepoMock.Object
                );

            var resultado = await casoDeUso.EjecutarAsync(proveedorId, restauranteId);

            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoInsumoCriticoNoLoVendeElProveedor_NoLoDevuelve()
        {
            int proveedorId = 1;
            int restauranteId = 1;
            var insumosProveedor = new List<DOM.Insumo>
            {
                new DOM.Insumo { Id = 10, Nombre = "Tomate"},
            };
            var insumosResto = new List<DOM.Insumo>
            {
                new DOM.Insumo { Id = 99, Nombre = "Cebolla", StockMinimo = 10},
            };
            _insumoRepoMock
                .Setup(r => r.ObtenerInsumosDelProveedorAsync(proveedorId, restauranteId))
                .ReturnsAsync(insumosProveedor);
            _insumoRepoMock
                .Setup(r => r.ObtenerInsumosAsync(restauranteId))
                .ReturnsAsync(insumosResto);
            _loteRepoMock
                .Setup(r => r.ObtenerStockTotalDeInsumo(99))
                .ReturnsAsync(2);
            _estadoInsumoServicioMock
                .Setup(r => r.CalcularEstadoStock(2, 10))
                .Returns(EstadoStock.Critico);

            var casoDeUso = new ObtenerInsumosParaPedidoCasoDeUso(
                _insumoRepoMock.Object,
                _estadoInsumoServicioMock.Object,
                _loteRepoMock.Object,
                _pedidoRepoMock.Object
                );

            var resultado = await casoDeUso.EjecutarAsync(proveedorId, restauranteId);

            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }
    }
}
