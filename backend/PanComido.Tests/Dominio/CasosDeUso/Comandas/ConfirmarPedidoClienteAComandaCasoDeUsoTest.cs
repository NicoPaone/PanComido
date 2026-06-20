using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.ComandaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Tests.Dominio.CasosDeUso.Comandas
{
    public class ConfirmarPedidoClienteAComandaCasoDeUsoTest
    {
        private readonly Mock<IComandaRepositorio> _comandaRepoMock;
        private readonly Mock<ILoteRepositorio> _loteRepoMock;
        private readonly Mock<IArticuloRepositorio> _articuloRepoMock;
        private readonly Mock<IMesaRepositorio> _mesaRepoMock;
        private readonly Mock<IDisponibilidadArticuloServicio> _disponibilidadServicioMock;
        private readonly Mock<IGestionStockServicio> _gestionStockServicioMock;
        private readonly Mock<IComandaNotificador> _comandaNotificadorMock;
        private readonly Mock<ILogger<ConfirmarPedidoClienteAComandaCasoDeUso>> _loggerMock;

        public ConfirmarPedidoClienteAComandaCasoDeUsoTest()
        {
            _comandaRepoMock = new Mock<IComandaRepositorio>();
            _loteRepoMock = new Mock<ILoteRepositorio>();
            _articuloRepoMock = new Mock<IArticuloRepositorio>();
            _mesaRepoMock = new Mock<IMesaRepositorio>();
            _disponibilidadServicioMock = new Mock<IDisponibilidadArticuloServicio>();
            _gestionStockServicioMock = new Mock<IGestionStockServicio>();
            _comandaNotificadorMock = new Mock<IComandaNotificador>();
            _loggerMock = new Mock<ILogger<ConfirmarPedidoClienteAComandaCasoDeUso>>();
        }

        private ConfirmarPedidoClienteAComandaCasoDeUso CrearCasoDeUsoConReposMock()
        {
            return new ConfirmarPedidoClienteAComandaCasoDeUso(
                _comandaRepoMock.Object,
                _loteRepoMock.Object,
                _articuloRepoMock.Object,
                _mesaRepoMock.Object,
                _disponibilidadServicioMock.Object,
                _gestionStockServicioMock.Object,
                _comandaNotificadorMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoComandaNoExisteOEstaFinalizada_LanzaInvalidOperationException()
        {
            // 1. Preparar
            ConfirmarPedidoClienteAComandaCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int restauranteId = 1;
            int comandaId = 99;
            List<ArticuloComanda> solicitados = new List<ArticuloComanda>();


            _comandaRepoMock.Setup(r => r.ObtenerComandaPorIdAsync(comandaId)).ReturnsAsync((Comanda)null);

            // 2. Ejecutar
            Func<Task> accion = async () => await casoDeUso.EjecutarAsync(restauranteId, comandaId, solicitados);

            // 3. Verificar
            var excepcion = await Assert.ThrowsAsync<InvalidOperationException>(accion);
            Assert.Equal("La comanda no existe o esta finalizada.", excepcion.Message);

            // no consulta stock general
            _loteRepoMock.Verify(r => r.ObtenerStockTotalDeInsumosDisponible(It.IsAny<int>(), It.IsAny<DateOnly>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoArticuloNoEsVisible_LanzaArgumentException()
        {
            // 1. Preparar
            ConfirmarPedidoClienteAComandaCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int restauranteId = 1;
            int comandaId = 1;

            Comanda comandaAbierta = new Comanda { Id = comandaId, Estado = EstadoComanda.Abierta };
            List<ArticuloComanda> solicitados = new List<ArticuloComanda>
            {
                new ArticuloComanda { ArticuloId = 5, Cantidad = 1 }
            };

            Articulo articuloOculto = new Articulo { Id = 5, EsVisibleEnCarta = false };

            _comandaRepoMock.Setup(r => r.ObtenerComandaPorIdAsync(comandaId)).ReturnsAsync(comandaAbierta);
            _articuloRepoMock.Setup(r => r.ObtenerDetalleAsync(restauranteId, 5)).ReturnsAsync(articuloOculto);

            // 2. Ejecutar
            Func<Task> accion = async () => await casoDeUso.EjecutarAsync(restauranteId, comandaId, solicitados);

            // 3. Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(accion);
            Assert.Equal("El artículo con ID 5 no está disponible.", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoQuiebraElStock_LanzaInvalidOperationException()
        {
            // 1. Preparar
            ConfirmarPedidoClienteAComandaCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int restauranteId = 1;
            int comandaId = 1;

            Comanda comandaAbierta = new Comanda { Id = comandaId, Estado = EstadoComanda.Abierta };
            List<ArticuloComanda> solicitados = new List<ArticuloComanda>
            {
                new ArticuloComanda { ArticuloId = 10, Cantidad = 5 }
            };

            Articulo articuloValido = new Articulo { Id = 10, Nombre = "Ensalada", EsVisibleEnCarta = true };
            Dictionary<int, decimal> stockActual = new Dictionary<int, decimal>();

            _comandaRepoMock.Setup(r => r.ObtenerComandaPorIdAsync(comandaId)).ReturnsAsync(comandaAbierta);
            _loteRepoMock.Setup(r => r.ObtenerStockTotalDeInsumosDisponible(restauranteId, It.IsAny<DateOnly>())).ReturnsAsync(stockActual);
            _articuloRepoMock.Setup(r => r.ObtenerDetalleAsync(restauranteId, 10)).ReturnsAsync(articuloValido);

            _disponibilidadServicioMock.Setup(s => s.VerificarDisponibilidad(articuloValido, 5, stockActual)).Returns(false);

            // 2. Ejecutar
            Func<Task> accion = async () => await casoDeUso.EjecutarAsync(restauranteId, comandaId, solicitados);

            // 3. Verificar
            var excepcion = await Assert.ThrowsAsync<InvalidOperationException>(accion);
            Assert.Equal("No hay stock suficiente para preparar 5x Ensalada", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTodoEsValido_ProcesaEscudosNotificaYDescuentaStock()
        {
            // 1. Preparar
            ConfirmarPedidoClienteAComandaCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int restauranteId = 1;
            int comandaId = 1;

            Comanda comandaAbierta = new Comanda { Id = comandaId, MesaId = 2, Estado = EstadoComanda.Abierta, Items = new List<ArticuloComanda>() };

            Plato hamburguesa = new Plato
            {
                Id = 4,
                EsVisibleEnCarta = true,
                Ingredientes = new List<PlatoIngrediente> { new PlatoIngrediente { InsumoId = 30, Cantidad = 1 } }
            };

            Insumo cocaCola = new Insumo { Id = 12, EsVisibleEnCarta = true };

            List<ArticuloComanda> solicitados = new List<ArticuloComanda>
            {
                new ArticuloComanda { ArticuloId = 4, Cantidad = 1, IngredientesExcluidosIds = new List<int> { 30, 30, 999 } }, // excluidos repetidos u erroneos
                new ArticuloComanda { ArticuloId = 12, Cantidad = 1, IngredientesExcluidosIds = new List<int> { 1, 2 } } // bebida con ingredientes excluidos erroneos
            };

            Dictionary<int, decimal> stockActual = new Dictionary<int, decimal>
            {
                { 30, 5m }, // hay cebolla
                { 12, 10m } // hay coca cola
            };

            // Mocks de lectura
            _comandaRepoMock.Setup(r => r.ObtenerComandaPorIdAsync(comandaId)).ReturnsAsync(comandaAbierta);
            _loteRepoMock.Setup(r => r.ObtenerStockTotalDeInsumosDisponible(restauranteId, It.IsAny<DateOnly>())).ReturnsAsync(stockActual);
            _articuloRepoMock.Setup(r => r.ObtenerDetalleAsync(restauranteId, 4)).ReturnsAsync(hamburguesa);
            _articuloRepoMock.Setup(r => r.ObtenerDetalleAsync(restauranteId, 12)).ReturnsAsync(cocaCola);

            _disponibilidadServicioMock.Setup(s => s.VerificarDisponibilidad(It.IsAny<Articulo>(), It.IsAny<int>(), stockActual)).Returns(true);
            _mesaRepoMock.Setup(r => r.ObtenerMozoIdsPorMesaAsync(2)).ReturnsAsync(new List<int> { 1 });

            // 2. Ejecutar
            Comanda resultado = await casoDeUso.EjecutarAsync(restauranteId, comandaId, solicitados);

            // 3. Verificar

            // hamburguesa con cebolla excluida
            Assert.Single(solicitados[0].IngredientesExcluidosIds);
            Assert.Contains(30, solicitados[0].IngredientesExcluidosIds);

            // bebida
            Assert.Empty(solicitados[1].IngredientesExcluidosIds);

            Assert.Equal(EstadoComanda.Nueva, resultado.Estado);

            _comandaRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Comanda>()), Times.Once);
            _comandaNotificadorMock.Verify(n => n.NotificarEstadoModificadoAsync(It.IsAny<Comanda>(), It.IsAny<List<int>>()), Times.Once);
            _comandaNotificadorMock.Verify(n => n.NotificarComandaActualizadaAMesaAsync(It.IsAny<Comanda>()), Times.Once);
            _gestionStockServicioMock.Verify(s => s.DescontarStockPorArticulosAsync(restauranteId, solicitados), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoIngredienteNoEsExcluido_RestaCantidadDelStockEnMemoria()
        {
            // 1. Preparar
            ConfirmarPedidoClienteAComandaCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int restauranteId = 1;
            int comandaId = 1;

            Comanda comandaAbierta = new Comanda { Id = comandaId, MesaId = 2, Estado = EstadoComanda.Abierta, Items = new List<ArticuloComanda>() };

           
            Plato hamburguesaDoble = new Plato
            {
                Id = 4,
                EsVisibleEnCarta = true,
                Ingredientes = new List<PlatoIngrediente> { new PlatoIngrediente { InsumoId = 10, Cantidad = 2 } }
            };

            // (total esperado a restar: 3 * 2 = 6 fetas)
            
            List<ArticuloComanda> solicitados = new List<ArticuloComanda>
            {
                new ArticuloComanda { ArticuloId = 4, Cantidad = 3, IngredientesExcluidosIds = new List<int>() } // excluidos vacia, osea pidio con todo
            };

            Dictionary<int, decimal> stockActualEnMemoria = new Dictionary<int, decimal>
            {
                { 10, 10m }
            };

            _comandaRepoMock.Setup(r => r.ObtenerComandaPorIdAsync(comandaId)).ReturnsAsync(comandaAbierta);
            _loteRepoMock.Setup(r => r.ObtenerStockTotalDeInsumosDisponible(restauranteId, It.IsAny<DateOnly>())).ReturnsAsync(stockActualEnMemoria);
            _articuloRepoMock.Setup(r => r.ObtenerDetalleAsync(restauranteId, 4)).ReturnsAsync(hamburguesaDoble);
            _disponibilidadServicioMock.Setup(s => s.VerificarDisponibilidad(It.IsAny<Articulo>(), It.IsAny<int>(), stockActualEnMemoria)).Returns(true);
            _mesaRepoMock.Setup(r => r.ObtenerMozoIdsPorMesaAsync(2)).ReturnsAsync(new List<int> { 1 });

            // 2. Ejecutar
            await casoDeUso.EjecutarAsync(restauranteId, comandaId, solicitados);

            // 3. Verificar
            // 10 - 6 = 4 fetas quedan en stock en memoria
            Assert.Equal(4m, stockActualEnMemoria[10]);
        }
    }
}
