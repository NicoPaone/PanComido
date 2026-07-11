using Moq;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Dashboard
{
    public class ObtenerResumenSatisfaccionCasoDeUsoTest
    {
        private readonly Mock<IDashboardRepositorio> _mockRepositorio;
        private readonly ObtenerResumenSatisfaccionCasoDeUso _casoDeUso;

        public ObtenerResumenSatisfaccionCasoDeUsoTest()
        {
            _mockRepositorio = new Mock<IDashboardRepositorio>();
            _casoDeUso = new ObtenerResumenSatisfaccionCasoDeUso(_mockRepositorio.Object);
        }

        [Fact]
        public async Task EjecutarAsync_SinEncuestas_RetornaResumenVacio()
        {
            // Preparar
            int restauranteId = 1;
            var desde = new DateTime(2023, 1, 1);
            var hasta = new DateTime(2023, 1, 31);

            _mockRepositorio.Setup(x => x.ObtenerEncuestasPorPeriodoAsync(restauranteId, desde, hasta))
                .ReturnsAsync(new List<EncuestaSatisfaccion>());

            // Actuar
            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, desde, hasta);

            // Verificar
            Assert.Equal(0, resultado.TotalEncuestas);
            Assert.Equal(0, resultado.PromedioComida);
            Assert.Equal(0, resultado.PromedioLugar);
            Assert.Equal(0, resultado.PromedioAtencion);
            Assert.Equal(0, resultado.TotalDerivadosGoogleMaps);
            Assert.Equal(0, resultado.PorcentajeDerivados);
        }

        [Fact]
        public async Task EjecutarAsync_ConEncuestas_CalculaPromediosYPorcentajesCorrectamente()
        {
            // Preparar
            int restauranteId = 1;
            var desde = new DateTime(2023, 1, 1);
            var hasta = new DateTime(2023, 1, 31);

            var encuestas = new List<EncuestaSatisfaccion>
            {
                // Promedio: (5+5+5)/3 = 5 (Derivado a Google Maps)
                new EncuestaSatisfaccion { PuntuacionComida = 5, PuntuacionLugar = 5, PuntuacionMozo = 5 },
                
                // Promedio: (3+4+3)/3 = 3.33 (NO Derivado a Google Maps)
                new EncuestaSatisfaccion { PuntuacionComida = 3, PuntuacionLugar = 4, PuntuacionMozo = 3 },
                
                // Promedio: (4+4+4)/3 = 4 (Derivado a Google Maps)
                new EncuestaSatisfaccion { PuntuacionComida = 4, PuntuacionLugar = 4, PuntuacionMozo = 4 },
                
                // Promedio: (1+2+1)/3 = 1.33 (NO Derivado a Google Maps)
                new EncuestaSatisfaccion { PuntuacionComida = 1, PuntuacionLugar = 2, PuntuacionMozo = 1 }
            };

            _mockRepositorio.Setup(x => x.ObtenerEncuestasPorPeriodoAsync(restauranteId, desde, hasta))
                .ReturnsAsync(encuestas);

            // Actuar
            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, desde, hasta);

            // Verificar
            Assert.Equal(4, resultado.TotalEncuestas);
            
            // Promedio Comida = (5 + 3 + 4 + 1) / 4 = 13 / 4 = 3.25
            Assert.Equal(3.25, resultado.PromedioComida);
            
            // Promedio Lugar = (5 + 4 + 4 + 2) / 4 = 15 / 4 = 3.75
            Assert.Equal(3.75, resultado.PromedioLugar);
            
            // Promedio Atencion (Mozo) = (5 + 3 + 4 + 1) / 4 = 13 / 4 = 3.25
            Assert.Equal(3.25, resultado.PromedioAtencion);
            
            // 2 de 4 tienen promedio >= 4.0
            Assert.Equal(2, resultado.TotalDerivadosGoogleMaps);
            
            // 2 de 4 = 50%
            Assert.Equal(50.0, resultado.PorcentajeDerivados);
        }
    }
}
