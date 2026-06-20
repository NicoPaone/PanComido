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
    public class TiempoDePreparacionPlatoServicioTest
    {
        private readonly Mock<IMesaRepositorio> _mesaRepositorioMock;
        private readonly TiempoDePreparacionPlatoServicio _servicio;

        public TiempoDePreparacionPlatoServicioTest()
        {
            _mesaRepositorioMock = new Mock<IMesaRepositorio>();
            _servicio = new TiempoDePreparacionPlatoServicio(_mesaRepositorioMock.Object);
        }

        [Fact]
        public void CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas_Devuelve0_CuandoNoHayMesasOcupadas()
        {
            // Preparar
            _mesaRepositorioMock.Setup(r => r.ObtenerTodasAsync(1))
                .ReturnsAsync(new List<MesaConPosiciones>
                {
                    new MesaConPosiciones(),
                    new MesaConPosiciones(),
                    new MesaConPosiciones(),
                    new MesaConPosiciones(),
                    new MesaConPosiciones()
                });

            _mesaRepositorioMock.Setup(r => r.ObtenerOcupadasAsync(1))
                .ReturnsAsync(new List<MesaConPosiciones>());

            // Ejecutar
            var resultado = _servicio.CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas(1);

            // Verificar
            Assert.Equal(0, resultado);
        }

        [Fact]
        public void CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas_Devuelve5_CuandoLaOcupacionEsMenorOIgualAl30Porciento()
        {
            // Preparar
            _mesaRepositorioMock.Setup(r => r.ObtenerTodasAsync(1))
                .ReturnsAsync(new List<MesaConPosiciones>
                {
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones()
                });

            _mesaRepositorioMock.Setup(r => r.ObtenerOcupadasAsync(1))
                .ReturnsAsync(new List<MesaConPosiciones>
                {
                    new MesaConPosiciones(),
                    new MesaConPosiciones(),
                    new MesaConPosiciones()
                });

            // Ejecutar
            var resultado = _servicio.CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas(1);

            // Verificar
            Assert.Equal(5, resultado);
        }

        [Fact]
        public void CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas_Devuelve10_CuandoLaOcupacionEsMenorOIgualAl50Porciento()
        {
            // Preparar
            _mesaRepositorioMock.Setup(r => r.ObtenerTodasAsync(1))
                .ReturnsAsync(new List<MesaConPosiciones>
                {
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones()
                });

            _mesaRepositorioMock.Setup(r => r.ObtenerOcupadasAsync(1))
                .ReturnsAsync(new List<MesaConPosiciones>
                {
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones()
                });

            // Ejecutar
            var resultado = _servicio.CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas(1);

            // Verificar
            Assert.Equal(10, resultado);
        }

        [Fact]
        public void CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas_Devuelve15_CuandoLaOcupacionEsMenorOIgualAl70Porciento()
        {
            // Preparar
            _mesaRepositorioMock.Setup(r => r.ObtenerTodasAsync(1))
                .ReturnsAsync(new List<MesaConPosiciones>
                {
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones()
                });

            _mesaRepositorioMock.Setup(r => r.ObtenerOcupadasAsync(1))
                .ReturnsAsync(new List<MesaConPosiciones>
                {
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones()
                });

            // Ejecutar
            var resultado = _servicio.CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas(1);

            // Verificar
            Assert.Equal(15, resultado);
        }

        [Fact]
        public void CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas_Devuelve20_CuandoLaOcupacionEsMayorAl70Porciento()
        {
            // Preparar
            _mesaRepositorioMock.Setup(r => r.ObtenerTodasAsync(1))
                .ReturnsAsync(new List<MesaConPosiciones>
                {
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones()
                });

            _mesaRepositorioMock.Setup(r => r.ObtenerOcupadasAsync(1))
                .ReturnsAsync(new List<MesaConPosiciones>
                {
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones()
                });

            // Ejecutar
            var resultado = _servicio.CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas(1);

            // Verificar
            Assert.Equal(20, resultado);
        }

        [Fact]
        public void CalcularTiempoPreparacionDinamico_SumaElTiempoBaseYElTiempoExtra()
        {
            // Preparar
            _mesaRepositorioMock.Setup(r => r.ObtenerTodasAsync(1))
                .ReturnsAsync(new List<MesaConPosiciones>
                {
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones(),
                    new MesaConPosiciones(), new MesaConPosiciones()
                });

            _mesaRepositorioMock.Setup(r => r.ObtenerOcupadasAsync(1))
                .ReturnsAsync(new List<MesaConPosiciones>
                {
                    new MesaConPosiciones(),
                    new MesaConPosiciones(),
                    new MesaConPosiciones()
                });

            var plato = new Plato
            {
                RestauranteId = 1,
                TiempoPreparacionBase = 20
            };

            // Ejecutar
            var resultado = _servicio.CalcularTiempoPreparacionDinamico(plato);

            // Verificar
            Assert.Equal(25, resultado);
        }
    }
}