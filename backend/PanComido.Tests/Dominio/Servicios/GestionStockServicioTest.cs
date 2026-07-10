using Moq;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Tests.Dominio.Servicios
{
    public class GestionStockServicioTest
    {
        private readonly Mock<ILoteRepositorio> _loteRepositorioMock;
        private readonly Mock<IInsumoRepositorio> _insumoRepositorioMock;
        private readonly GestionStockServicio _servicio;

        public GestionStockServicioTest()
        {
            _loteRepositorioMock = new Mock<ILoteRepositorio>();
            _insumoRepositorioMock = new Mock<IInsumoRepositorio>();
            _servicio = new GestionStockServicio(_loteRepositorioMock.Object, _insumoRepositorioMock.Object);
        }

        [Fact]
        public async Task DescontarStockPorArticulosAsync_CuandoEsBebida_DescuentaStockDelLote()
        {
            // Preparar
            var bebida = new Insumo
            {
                Id = 1
            };

            var comanda = new ArticuloComanda
            {
                Articulo = bebida,
                Cantidad = 2,
                IngredientesExcluidosIds = new List<int>()
            };

            var lote = new Lote
            {
                Id = 1,
                Cantidad = 10
            };

            _loteRepositorioMock
                .Setup(r => r.ObtenerLotesPorFechaVencimientoAscendenteAsync(1, 1))
                .ReturnsAsync(new List<Lote> { lote });

            // Ejecutar
            await _servicio.DescontarStockPorArticulosAsync(
                1,
                new List<ArticuloComanda> { comanda });

            // Verificar
            Assert.Equal(8, lote.Cantidad);

            _loteRepositorioMock.Verify(
                r => r.ActualizarLotesAsync(It.IsAny<List<Lote>>()),
                Times.Once);
        }

        [Fact]
        public async Task DescontarStockPorArticulosAsync_CuandoEsPlato_DescuentaIngredientes()
        {
            // Preparar
            var plato = new Plato
            {
                Ingredientes = new List<PlatoIngrediente>
                {
                    new PlatoIngrediente
                    {
                        InsumoId = 1,
                        Cantidad = 2
                    }
                }
            };

            var comanda = new ArticuloComanda
            {
                Articulo = plato,
                Cantidad = 3,
                IngredientesExcluidosIds = new List<int>()
            };

            var lote = new Lote
            {
                Id = 1,
                Cantidad = 20
            };

            _loteRepositorioMock
                .Setup(r => r.ObtenerLotesPorFechaVencimientoAscendenteAsync(1, 1))
                .ReturnsAsync(new List<Lote> { lote });

            // Ejecutar
            await _servicio.DescontarStockPorArticulosAsync(
                1,
                new List<ArticuloComanda> { comanda });

            // Verificar
            Assert.Equal(14, lote.Cantidad);
        }

        [Fact]
        public async Task DescontarStockPorArticulosAsync_NoDescuentaIngredientesExcluidos()
        {
            // Preparar
            var plato = new Plato
            {
                Ingredientes = new List<PlatoIngrediente>
                {
                    new PlatoIngrediente
                    {
                        InsumoId = 1,
                        Cantidad = 2
                    }
                }
            };

            var comanda = new ArticuloComanda
            {
                Articulo = plato,
                Cantidad = 1,
                IngredientesExcluidosIds = new List<int> { 1 }
            };

            // Ejecutar
            await _servicio.DescontarStockPorArticulosAsync(
                1,
                new List<ArticuloComanda> { comanda });

            // Verificar
            _loteRepositorioMock.Verify(
                r => r.ObtenerLotesPorFechaVencimientoAscendenteAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>()),
                Times.Never);

            _loteRepositorioMock.Verify(
                r => r.ActualizarLotesAsync(It.IsAny<List<Lote>>()),
                Times.Never);
        }

        [Fact]
        public async Task DescontarStockPorArticulosAsync_ConsumeLotesEnOrdenFIFO()
        {
            // Preparar
            var bebida = new Insumo
            {
                Id = 1
            };

            var comanda = new ArticuloComanda
            {
                Articulo = bebida,
                Cantidad = 15,
                IngredientesExcluidosIds = new List<int>()
            };

            var lote1 = new Lote
            {
                Id = 1,
                Cantidad = 10
            };

            var lote2 = new Lote
            {
                Id = 2,
                Cantidad = 20
            };

            _loteRepositorioMock
                .Setup(r => r.ObtenerLotesPorFechaVencimientoAscendenteAsync(1, 1))
                .ReturnsAsync(new List<Lote>
                {
                    lote1,
                    lote2
                });

            // Ejecutar
            await _servicio.DescontarStockPorArticulosAsync(
                1,
                new List<ArticuloComanda> { comanda });

            // Verificar
            Assert.Equal(0, lote1.Cantidad);
            Assert.Equal(15, lote2.Cantidad);
        }

        [Fact]
        public async Task DescontarStockPorArticulosAsync_CuandoEsBebidaPreparada_DescuentaInsumosDeLaReceta()
        {
            // Preparar
            var bebidaPreparada = new BebidaPreparada
            {
                Insumos = new List<BebidaPreparadaInsumo>
                {
                    new BebidaPreparadaInsumo { InsumoId = 1, Cantidad = 100 },
                    new BebidaPreparadaInsumo { InsumoId = 2, Cantidad = 300 }
                }
            };

            var comanda = new ArticuloComanda
            {
                Articulo = bebidaPreparada,
                Cantidad = 2,
                IngredientesExcluidosIds = new List<int>()
            };

            var loteFernet = new Lote { Id = 1, Cantidad = 1000 };
            var loteCoca = new Lote { Id = 2, Cantidad = 2000 };

            _loteRepositorioMock
                .Setup(r => r.ObtenerLotesPorFechaVencimientoAscendenteAsync(1, 1))
                .ReturnsAsync(new List<Lote> { loteFernet });
            _loteRepositorioMock
                .Setup(r => r.ObtenerLotesPorFechaVencimientoAscendenteAsync(1, 2))
                .ReturnsAsync(new List<Lote> { loteCoca });

            // Ejecutar
            await _servicio.DescontarStockPorArticulosAsync(
                1,
                new List<ArticuloComanda> { comanda });

            // Verificar
            Assert.Equal(800, loteFernet.Cantidad);
            Assert.Equal(1400, loteCoca.Cantidad);
        }

        [Fact]
        public async Task DescontarStockPorArticulosAsync_NoActualizaLotes_CuandoNoHayArticulos()
        {
            // Ejecutar
            await _servicio.DescontarStockPorArticulosAsync(
                1,
                new List<ArticuloComanda>());

            // Verificar
            _loteRepositorioMock.Verify(
                r => r.ActualizarLotesAsync(It.IsAny<List<Lote>>()),
                Times.Never);
        }
    }
}