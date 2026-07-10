using Moq;
using PanComido.Dominio.CasosDeUso.CartaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Tests.Dominio.CasosDeUso.Carta
{
    public class ObtenerCartaComensalCasoDeUsoTest
    {
        private readonly Mock<IArticuloRepositorio> _articuloRepoMock;
        private readonly Mock<ILoteRepositorio> _loteRepoMock;
        private readonly Mock<IDisponibilidadArticuloServicio> _dispoServicioMock;
        private readonly Mock<ITiempoDePreparacionPlatoServicio> _tiempoServicioMock;

        public ObtenerCartaComensalCasoDeUsoTest()
        {
            _articuloRepoMock = new Mock<IArticuloRepositorio>();
            _loteRepoMock = new Mock<ILoteRepositorio>();
            _dispoServicioMock = new Mock<IDisponibilidadArticuloServicio>();
            _tiempoServicioMock = new Mock<ITiempoDePreparacionPlatoServicio>();
        }

        private ObtenerCartaComensalCasoDeUso CrearCasoDeUsoConReposMock()
        {
            return new ObtenerCartaComensalCasoDeUso(
                _articuloRepoMock.Object,
                _loteRepoMock.Object,
                _dispoServicioMock.Object,
                _tiempoServicioMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoHayNingunArticuloEnLaDB_RetornaListaVacia()
        {
            // 1. Preparar
            ObtenerCartaComensalCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int restauranteId = 1;

            _articuloRepoMock.Setup(r => r.ObtenerArticulosEnCartaConIngredientesAsync(restauranteId))
                             .ReturnsAsync(new List<Articulo>());

            _loteRepoMock.Setup(r => r.ObtenerStockTotalDeInsumosDisponible(restauranteId, It.IsAny<DateOnly>()))
                         .ReturnsAsync(new Dictionary<int, decimal>());

            // 2. Ejecutar
            List<Articulo> resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // 3. Verificar
            Assert.NotNull(resultado);
            Assert.Empty(resultado); 

            // no se intento calcular ningun tiempo
            _tiempoServicioMock.Verify(s => s.CalcularTiempoPreparacionDinamico(It.IsAny<Plato>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHayArticulosPeroNoHayStock_RetornaListaVacia()
        {
            // 1. Preparar
            ObtenerCartaComensalCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int restauranteId = 1;

            Plato pizza = new Plato { Id = 1, Nombre = "Pizza Muzzarella" };
            List<Articulo> articulosDb = new List<Articulo> { pizza };
            Dictionary<int, decimal> stockVacio = new Dictionary<int, decimal>();

            _articuloRepoMock.Setup(r => r.ObtenerArticulosEnCartaConIngredientesAsync(restauranteId))
                             .ReturnsAsync(articulosDb);

            _loteRepoMock.Setup(r => r.ObtenerStockTotalDeInsumosDisponible(restauranteId, It.IsAny<DateOnly>()))
                         .ReturnsAsync(stockVacio);

            
            _dispoServicioMock.Setup(s => s.VerificarDisponibilidad(pizza, stockVacio)).Returns(false);

            // 2. Ejecutar
            List<Articulo> resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // 3. Verificar
            Assert.Empty(resultado);

            // sin stock, no se intento calcular ningun tiempo
            _tiempoServicioMock.Verify(s => s.CalcularTiempoPreparacionDinamico(It.IsAny<Plato>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoHayBebidasYPlatosDisponibles_CalculaTiempoSoloParaPlatos()
        {
            // 1. Preparar
            ObtenerCartaComensalCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int restauranteId = 1;

            Plato hamburguesa = new Plato { Id = 10, Nombre = "Hamburguesa Simple" };
            Insumo cocaCola = new Insumo { Id = 20, Nombre = "Coca Cola" }; 

            List<Articulo> articulosDb = new List<Articulo> { hamburguesa, cocaCola };
            Dictionary<int, decimal> stockActual = new Dictionary<int, decimal>();

            _articuloRepoMock.Setup(r => r.ObtenerArticulosEnCartaConIngredientesAsync(restauranteId))
                             .ReturnsAsync(articulosDb);

            _loteRepoMock.Setup(r => r.ObtenerStockTotalDeInsumosDisponible(restauranteId, It.IsAny<DateOnly>()))
                         .ReturnsAsync(stockActual);

            
            _dispoServicioMock.Setup(s => s.VerificarDisponibilidad(hamburguesa, stockActual)).Returns(true);
            _dispoServicioMock.Setup(s => s.VerificarDisponibilidad(cocaCola, stockActual)).Returns(true);

            
            _tiempoServicioMock.Setup(s => s.CalcularTiempoPreparacionDinamico(hamburguesa)).ReturnsAsync(15);

            // 2. Ejecutar
            List<Articulo> resultado = await casoDeUso.EjecutarAsync(restauranteId);

            // 3. Verificar
            Assert.Equal(2, resultado.Count); 

          
            Plato platoResultado = (Plato)resultado.Find(a => a.Id == 10);
            Assert.Equal(15, platoResultado.TiempoPreparacionEstimado);

            // solo se llama al tiempo para el plato, no para la bebida
            _tiempoServicioMock.Verify(s => s.CalcularTiempoPreparacionDinamico(It.IsAny<Plato>()), Times.Once);
        }
    }
}
