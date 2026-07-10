using Moq;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.Servicios
{
    public class TiempoDePreparacionPlatoServicioTest
    {
        private readonly Mock<IMesaRepositorio> _mesaRepositorioMock;
        private readonly Mock<IReglaTiempoExtraRepositorio> _reglaRepositorioMock;
        private readonly TiempoDePreparacionPlatoServicio _servicio;

        public TiempoDePreparacionPlatoServicioTest()
        {
            _mesaRepositorioMock = new Mock<IMesaRepositorio>();
            _reglaRepositorioMock = new Mock<IReglaTiempoExtraRepositorio>();
            _servicio = new TiempoDePreparacionPlatoServicio(_mesaRepositorioMock.Object, _reglaRepositorioMock.Object);
            
            _reglaRepositorioMock.Setup(r => r.ObtenerPorRestauranteIdAsync(1))
                .ReturnsAsync(new List<ReglaTiempoExtra>
                {
                    new ReglaTiempoExtra { PorcentajeOcupacionHasta = 30, MinutosExtra = 5 },
                    new ReglaTiempoExtra { PorcentajeOcupacionHasta = 50, MinutosExtra = 10 },
                    new ReglaTiempoExtra { PorcentajeOcupacionHasta = 70, MinutosExtra = 15 },
                    new ReglaTiempoExtra { PorcentajeOcupacionHasta = 100, MinutosExtra = 20 }
                });
        }

        [Fact]
        public async Task CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas_Devuelve0_CuandoNoHayMesasOcupadas()
        {
            // Preparar
            _mesaRepositorioMock.Setup(r => r.ObtenerTodasAsync(1))
                .ReturnsAsync(new List<MesaConPosiciones>
                {
                    new MesaConPosiciones(), new MesaConPosiciones(), new MesaConPosiciones(), new MesaConPosiciones(), new MesaConPosiciones()
                });

            _mesaRepositorioMock.Setup(r => r.ObtenerOcupadasAsync(1))
                .ReturnsAsync(new List<MesaConPosiciones>());

            // Ejecutar
            var resultado = await _servicio.CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas(1);

            // Verificar
            Assert.Equal(0, resultado);
        }

        [Fact]
        public async Task CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas_Devuelve5_CuandoLaOcupacionEsMenorOIgualAl30Porciento()
        {
            // Preparar
            _mesaRepositorioMock.Setup(r => r.ObtenerTodasAsync(1))
                .ReturnsAsync(Enumerable.Repeat(new MesaConPosiciones(), 10).ToList());

            _mesaRepositorioMock.Setup(r => r.ObtenerOcupadasAsync(1))
                .ReturnsAsync(Enumerable.Repeat(new MesaConPosiciones(), 3).ToList());

            // Ejecutar
            var resultado = await _servicio.CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas(1);

            // Verificar
            Assert.Equal(5, resultado);
        }

        [Fact]
        public async Task CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas_Devuelve10_CuandoLaOcupacionEsMenorOIgualAl50Porciento()
        {
            // Preparar
            _mesaRepositorioMock.Setup(r => r.ObtenerTodasAsync(1))
                .ReturnsAsync(Enumerable.Repeat(new MesaConPosiciones(), 10).ToList());

            _mesaRepositorioMock.Setup(r => r.ObtenerOcupadasAsync(1))
                .ReturnsAsync(Enumerable.Repeat(new MesaConPosiciones(), 5).ToList());

            // Ejecutar
            var resultado = await _servicio.CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas(1);

            // Verificar
            Assert.Equal(10, resultado);
        }

        [Fact]
        public async Task CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas_Devuelve15_CuandoLaOcupacionEsMenorOIgualAl70Porciento()
        {
            // Preparar
            _mesaRepositorioMock.Setup(r => r.ObtenerTodasAsync(1))
                .ReturnsAsync(Enumerable.Repeat(new MesaConPosiciones(), 10).ToList());

            _mesaRepositorioMock.Setup(r => r.ObtenerOcupadasAsync(1))
                .ReturnsAsync(Enumerable.Repeat(new MesaConPosiciones(), 7).ToList());

            // Ejecutar
            var resultado = await _servicio.CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas(1);

            // Verificar
            Assert.Equal(15, resultado);
        }

        [Fact]
        public async Task CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas_Devuelve20_CuandoLaOcupacionEsMayorAl70Porciento()
        {
            // Preparar
            _mesaRepositorioMock.Setup(r => r.ObtenerTodasAsync(1))
                .ReturnsAsync(Enumerable.Repeat(new MesaConPosiciones(), 10).ToList());

            _mesaRepositorioMock.Setup(r => r.ObtenerOcupadasAsync(1))
                .ReturnsAsync(Enumerable.Repeat(new MesaConPosiciones(), 8).ToList());

            // Ejecutar
            var resultado = await _servicio.CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas(1);

            // Verificar
            Assert.Equal(20, resultado);
        }

        [Fact]
        public async Task CalcularTiempoPreparacionDinamico_SumaElTiempoBaseYElTiempoExtra()
        {
            // Preparar
            _mesaRepositorioMock.Setup(r => r.ObtenerTodasAsync(1))
                .ReturnsAsync(Enumerable.Repeat(new MesaConPosiciones(), 10).ToList());

            _mesaRepositorioMock.Setup(r => r.ObtenerOcupadasAsync(1))
                .ReturnsAsync(Enumerable.Repeat(new MesaConPosiciones(), 3).ToList());

            var plato = new Plato
            {
                RestauranteId = 1,
                TiempoPreparacionBase = 20
            };

            // Ejecutar
            var resultado = await _servicio.CalcularTiempoPreparacionDinamico(plato);

            // Verificar
            Assert.Equal(25, resultado);
        }
    }
}