using PanComido.Dominio.Interfaces.Servicios.IA;
using Moq;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.IA;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Repositorios.IA;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Dashboard
{
    public class ObtenerAnalisisPlatoCasoDeUsoTest
    {
        private readonly Mock<IPlatoAnalisisRepositorio> _platoAnalisisRepoMock;
        private readonly Mock<ISugerenciaIARepositorio> _sugerenciaIaRepoMock;
        private readonly Mock<ICalculadorCostoPlatoServicio> _calculadorCostoMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly Mock<ISugerenciaPlatosIAServicio> _sugerenciaPlatosIAServicioMock;
        private readonly ObtenerAnalisisPlatoCasoDeUso _casoDeUso;

        public ObtenerAnalisisPlatoCasoDeUsoTest()
        {
            _platoAnalisisRepoMock = new Mock<IPlatoAnalisisRepositorio>();
            _sugerenciaIaRepoMock = new Mock<ISugerenciaIARepositorio>();
            _calculadorCostoMock = new Mock<ICalculadorCostoPlatoServicio>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _sugerenciaPlatosIAServicioMock = new Mock<ISugerenciaPlatosIAServicio>();
            _casoDeUso = new ObtenerAnalisisPlatoCasoDeUso(
                _platoAnalisisRepoMock.Object, 
                _sugerenciaIaRepoMock.Object,
                _calculadorCostoMock.Object,
                _dateTimeProviderMock.Object,
                _sugerenciaPlatosIAServicioMock.Object);
        }


        [Fact]
        public async Task EjecutarAsync_DebeRetornarNull_CuandoNoExistePlato()
        {
            int restauranteId = 1;
            string nombrePlato = "NoExiste";
            _platoAnalisisRepoMock.Setup(r => r.ObtenerArticuloConPlatoYIngredientesPorNombreAsync(restauranteId, nombrePlato))
                .ReturnsAsync((Articulo?)null);

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, nombrePlato);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task EjecutarAsync_DebeCalcularMetricasYSugerenciasCorrectamente()
        {
            int restauranteId = 1;
            string nombrePlato = "Papas Fritas";
            var plato = new Plato
            {
                Id = 10,
                Nombre = nombrePlato,
                PrecioVentaFinal = 4000,
                CategoriaPlatoId = 2,
                Ingredientes = new List<PlatoIngrediente>
                {
                    new PlatoIngrediente { InsumoId = 101, Cantidad = 0.5m }
                }
            };

            var fechaReferencia = new DateTime(2023, 1, 1);
            _dateTimeProviderMock.Setup(d => d.ObtenerAhora()).Returns(fechaReferencia);
            _dateTimeProviderMock.Setup(d => d.ObtenerHoy()).Returns(fechaReferencia.Date);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerArticuloConPlatoYIngredientesPorNombreAsync(restauranteId, nombrePlato))
                .ReturnsAsync(plato);

            _calculadorCostoMock.Setup(c => c.CalcularCostoAsync(plato))
                .ReturnsAsync(1000m);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerVentasArticuloEnRangoAsync(restauranteId, plato.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(15);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerVentasCategoriaEnRangoAsync(restauranteId, plato.CategoriaPlatoId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(100);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerPlatoLiderDeCategoriaAsync(restauranteId, plato.CategoriaPlatoId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new RendimientoPlato { PlatoId = 20, Nombre = "Papas Rusticas", UnidadesVendidas = 45, FacturacionTotal = 202500m });

            _platoAnalisisRepoMock.Setup(r => r.ObtenerVentasSemanalesArticuloAsync(restauranteId, plato.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<int> { 0, 1, 2, 3, 4, 5, 6 });

            _sugerenciaIaRepoMock.Setup(r => r.ObtenerSugerenciaIAAsync(restauranteId))
                .ReturnsAsync((SugerenciaIA?)null);

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, nombrePlato);

            Assert.NotNull(resultado);
            Assert.Equal(plato, resultado.Articulo);
            Assert.Equal(1000m, resultado.CostoPreparacion);
            Assert.Equal(15, resultado.VentasPeriodo);
            Assert.True(resultado.Participacion == "15,0%" || resultado.Participacion == "15.0%", $"Participacion actual: {resultado.Participacion}");
            Assert.Equal("Papas Rusticas", resultado.ComparativaLider.Nombre);
            Assert.Equal(7, resultado.Tendencia.Count);
            Assert.NotNull(resultado.AnalisisIa);
            Assert.Equal(2, resultado.AnalisisIa.Sugerencias.Count);
            
            _sugerenciaIaRepoMock.Verify(r => r.GuardarSugerenciaIAAsync(restauranteId, It.IsAny<SugerenciaIA>()), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_DebeUsarMockFallback_CuandoVentasPeriodoEsCero()
        {
            int restauranteId = 1;
            string nombrePlato = "Ensalada Cesar";
            var plato = new Plato
            {
                Id = 11,
                Nombre = nombrePlato,
                PrecioVentaFinal = 3500,
                CategoriaPlatoId = 2
            };

            var fechaReferencia = new DateTime(2023, 1, 1);
            _dateTimeProviderMock.Setup(d => d.ObtenerAhora()).Returns(fechaReferencia);
            _dateTimeProviderMock.Setup(d => d.ObtenerHoy()).Returns(fechaReferencia.Date);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerArticuloConPlatoYIngredientesPorNombreAsync(restauranteId, nombrePlato))
                .ReturnsAsync(plato);

            _calculadorCostoMock.Setup(c => c.CalcularCostoAsync(plato)).ReturnsAsync(900m);

            // 0 Ventas
            _platoAnalisisRepoMock.Setup(r => r.ObtenerVentasArticuloEnRangoAsync(restauranteId, plato.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(0);

            _sugerenciaIaRepoMock.Setup(r => r.ObtenerSugerenciaIAAsync(restauranteId))
                .ReturnsAsync((SugerenciaIA?)null);

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, nombrePlato);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.AnalisisIa);
            // El mock fallback devuelve diagnóstico con el nombre del plato
            Assert.Contains(nombrePlato, resultado.AnalisisIa.Diagnostico);
            Assert.Equal(2, resultado.AnalisisIa.Sugerencias.Count);
            // No se debe haber llamado a la IA porque ventas = 0
            _sugerenciaPlatosIAServicioMock.Verify(s => s.AnalizarPlatoRendimientoAsync(
                It.IsAny<Plato>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<RendimientoPlato>(), It.IsAny<List<int>>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_DebeUsarMockFallback_CuandoServicioIaLanzaExcepcion()
        {
            int restauranteId = 1;
            string nombrePlato = "Burguer XL";
            var plato = new Plato
            {
                Id = 12,
                Nombre = nombrePlato,
                PrecioVentaFinal = 5000,
                CategoriaPlatoId = 2
            };

            var fechaReferencia = new DateTime(2023, 1, 1);
            _dateTimeProviderMock.Setup(d => d.ObtenerAhora()).Returns(fechaReferencia);
            _dateTimeProviderMock.Setup(d => d.ObtenerHoy()).Returns(fechaReferencia.Date);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerArticuloConPlatoYIngredientesPorNombreAsync(restauranteId, nombrePlato))
                .ReturnsAsync(plato);

            _calculadorCostoMock.Setup(c => c.CalcularCostoAsync(plato)).ReturnsAsync(1500m);

            // Tiene ventas
            _platoAnalisisRepoMock.Setup(r => r.ObtenerVentasArticuloEnRangoAsync(restauranteId, plato.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(10);

            _sugerenciaIaRepoMock.Setup(r => r.ObtenerSugerenciaIAAsync(restauranteId))
                .ReturnsAsync((SugerenciaIA?)null);

            // Servicio de IA falla
            _sugerenciaPlatosIAServicioMock.Setup(s => s.AnalizarPlatoRendimientoAsync(
                It.IsAny<Plato>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<RendimientoPlato>(), It.IsAny<List<int>>()))
                .ThrowsAsync(new Exception("API Down"));

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, nombrePlato);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.AnalisisIa);
            // Debe caer en el mock fallback
            Assert.Contains(nombrePlato, resultado.AnalisisIa.Diagnostico);
            Assert.Equal(2, resultado.AnalisisIa.Sugerencias.Count);
        }

        [Fact]
        public async Task EjecutarAsync_DebeUsarServicioIa_CuandoHayVentasYIaEsExitosa()
        {
            int restauranteId = 1;
            string nombrePlato = "Sushi Roll";
            var plato = new Plato
            {
                Id = 13,
                Nombre = nombrePlato,
                PrecioVentaFinal = 6000,
                CategoriaPlatoId = 2
            };

            var fechaReferencia = new DateTime(2023, 1, 1);
            _dateTimeProviderMock.Setup(d => d.ObtenerAhora()).Returns(fechaReferencia);
            _dateTimeProviderMock.Setup(d => d.ObtenerHoy()).Returns(fechaReferencia.Date);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerArticuloConPlatoYIngredientesPorNombreAsync(restauranteId, nombrePlato))
                .ReturnsAsync(plato);

            _calculadorCostoMock.Setup(c => c.CalcularCostoAsync(plato)).ReturnsAsync(2000m);

            // Tiene ventas
            _platoAnalisisRepoMock.Setup(r => r.ObtenerVentasArticuloEnRangoAsync(restauranteId, plato.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(20);

            _sugerenciaIaRepoMock.Setup(r => r.ObtenerSugerenciaIAAsync(restauranteId))
                .ReturnsAsync((SugerenciaIA?)null);

            var analisisRealIa = new PlatoAnalisisIa
            {
                Diagnostico = "Diagnóstico optimizado por Gemini",
                Alerta = "moderada",
                Sugerencias = new List<PlatoSugerenciaIa>
                {
                    new PlatoSugerenciaIa { Id = 1, Tipo = "precio", Accion = "Subir precio 5%", Impacto = "Alto", Dificultad = "baja" }
                }
            };

            // Servicio de IA responde con éxito
            _sugerenciaPlatosIAServicioMock.Setup(s => s.AnalizarPlatoRendimientoAsync(
                It.IsAny<Plato>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<RendimientoPlato>(), It.IsAny<List<int>>()))
                .ReturnsAsync(analisisRealIa);

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, nombrePlato);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.AnalisisIa);
            // Debe retornar los datos de la IA
            Assert.Equal("Diagnóstico optimizado por Gemini", resultado.AnalisisIa.Diagnostico);
            Assert.Single(resultado.AnalisisIa.Sugerencias);
            Assert.Equal("Subir precio 5%", resultado.AnalisisIa.Sugerencias[0].Accion);
        }

        [Fact]
        public async Task EjecutarAsync_DebeUsarCache_CuandoElPlatoYaFueAnalizadoHoy()
        {
            int restauranteId = 1;
            string nombrePlato = "Sushi Roll";
            var plato = new Plato
            {
                Id = 13,
                Nombre = nombrePlato,
                PrecioVentaFinal = 6000,
                CategoriaPlatoId = 2
            };

            var fechaReferencia = new DateTime(2023, 1, 1);
            _dateTimeProviderMock.Setup(d => d.ObtenerAhora()).Returns(fechaReferencia);
            _dateTimeProviderMock.Setup(d => d.ObtenerHoy()).Returns(fechaReferencia.Date);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerArticuloConPlatoYIngredientesPorNombreAsync(restauranteId, nombrePlato))
                .ReturnsAsync(plato);

            _calculadorCostoMock.Setup(c => c.CalcularCostoAsync(plato)).ReturnsAsync(2000m);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerVentasArticuloEnRangoAsync(restauranteId, plato.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(20);

            // Ya fue analizado hoy (está en PlatosAnalisis)
            var sugerenciaExistente = new SugerenciaIA
            {
                FechaUltimoAnalisisIA = fechaReferencia,
                PlatosAnalisis = new List<PlatoAnalisisIa>
                {
                    new PlatoAnalisisIa { PlatoId = plato.Id, Nombre = plato.Nombre, Diagnostico = "Diagnóstico Cacheado" }
                }
            };

            _sugerenciaIaRepoMock.Setup(r => r.ObtenerSugerenciaIAAsync(restauranteId))
                .ReturnsAsync(sugerenciaExistente);

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, nombrePlato);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.AnalisisIa);
            Assert.Equal("Diagnóstico Cacheado", resultado.AnalisisIa.Diagnostico);
            
            _sugerenciaPlatosIAServicioMock.Verify(s => s.AnalizarPlatoRendimientoAsync(
                It.IsAny<Plato>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<RendimientoPlato>(), It.IsAny<List<int>>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_DebeResetearPlatosAnalisisYPermitirIA_CuandoEsOtroDia()
        {
            int restauranteId = 1;
            string nombrePlato = "Sushi Roll";
            var plato = new Plato
            {
                Id = 13,
                Nombre = nombrePlato,
                PrecioVentaFinal = 6000,
                CategoriaPlatoId = 2
            };

            var fechaAyer = new DateTime(2022, 12, 31);
            var fechaHoy = new DateTime(2023, 1, 1);
            _dateTimeProviderMock.Setup(d => d.ObtenerAhora()).Returns(fechaHoy);
            _dateTimeProviderMock.Setup(d => d.ObtenerHoy()).Returns(fechaHoy.Date);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerArticuloConPlatoYIngredientesPorNombreAsync(restauranteId, nombrePlato))
                .ReturnsAsync(plato);

            _calculadorCostoMock.Setup(c => c.CalcularCostoAsync(plato)).ReturnsAsync(2000m);

            _platoAnalisisRepoMock.Setup(r => r.ObtenerVentasArticuloEnRangoAsync(restauranteId, plato.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(20);

            // Sugerencia vieja (de ayer) con plato analizado ayer
            var sugerenciaExistente = new SugerenciaIA
            {
                FechaUltimoAnalisisIA = fechaAyer,
                PlatosAnalisis = new List<PlatoAnalisisIa>
                {
                    new PlatoAnalisisIa { PlatoId = plato.Id, Nombre = plato.Nombre, Diagnostico = "Ayer" }
                }
            };

            _sugerenciaIaRepoMock.Setup(r => r.ObtenerSugerenciaIAAsync(restauranteId))
                .ReturnsAsync(sugerenciaExistente);

            var analisisRealIa = new PlatoAnalisisIa
            {
                Diagnostico = "Diagnóstico hoy",
                Alerta = "moderada",
                Sugerencias = new List<PlatoSugerenciaIa>()
            };

            _sugerenciaPlatosIAServicioMock.Setup(s => s.AnalizarPlatoRendimientoAsync(
                It.IsAny<Plato>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<RendimientoPlato>(), It.IsAny<List<int>>()))
                .ReturnsAsync(analisisRealIa);

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, nombrePlato);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.AnalisisIa);
            // Debe resetear el plato de ayer y permitir la llamada hoy
            Assert.Equal("Diagnóstico hoy", resultado.AnalisisIa.Diagnostico);
            
            _sugerenciaPlatosIAServicioMock.Verify(s => s.AnalizarPlatoRendimientoAsync(
                It.IsAny<Plato>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<RendimientoPlato>(), It.IsAny<List<int>>()), Times.Once);
        }
    }
}

