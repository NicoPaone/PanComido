using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Configuracion
{
    public class ActualizarDatosTransferenciaCasoDeUsoTest
    {
        private readonly Mock<IDatosTransferenciaRepositorio> _mockRepositorio;
        private readonly Mock<ILogger<ActualizarDatosTransferenciaCasoDeUso>> _mockLogger;
        private readonly ActualizarDatosTransferenciaCasoDeUso _casoDeUso;

        public ActualizarDatosTransferenciaCasoDeUsoTest()
        {
            _mockRepositorio = new Mock<IDatosTransferenciaRepositorio>();
            _mockLogger = new Mock<ILogger<ActualizarDatosTransferenciaCasoDeUso>>();
            _casoDeUso = new ActualizarDatosTransferenciaCasoDeUso(_mockRepositorio.Object, _mockLogger.Object);
        }

        [Theory]
        [InlineData(null, "12345", "Titular")]
        [InlineData("Alias", null, "Titular")]
        [InlineData("Alias", "12345", null)]
        public async Task EjecutarAsync_DatosIncompletos_LanzaArgumentException(string? alias, string? numeroCuenta, string? titular)
        {
            // Preparar
            int restauranteId = 1;
            var datos = new DatosTransferencia 
            { 
                Alias = alias, 
                NumeroCuenta = numeroCuenta, 
                TitularCuenta = titular 
            };

            // Actuar & Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() => 
                _casoDeUso.EjecutarAsync(restauranteId, datos));

            Assert.Equal("El alias, numero de cuenta y titular no pueden estar vacios.", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_CbuConLongitudInvalida_LanzaArgumentException()
        {
            // Preparar
            int restauranteId = 1;
            var datos = new DatosTransferencia 
            { 
                Alias = "Alias", 
                NumeroCuenta = "12345", 
                TitularCuenta = "Titular",
                Cbu = "123" // Menos de 22 caracteres
            };

            // Actuar & Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() => 
                _casoDeUso.EjecutarAsync(restauranteId, datos));

            Assert.Equal("El CBU debe tener 22 caracteres.", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_DatosValidos_ActualizaYRetornaDatos()
        {
            // Preparar
            int restauranteId = 1;
            var datos = new DatosTransferencia 
            { 
                Alias = "MiAlias", 
                NumeroCuenta = "123456789", 
                TitularCuenta = "Juan Perez",
                Cbu = "1234567890123456789012" // 22 caracteres exactos
            };

            _mockRepositorio.Setup(x => x.ActualizarDatosTransferenciaAsync(restauranteId, datos))
                .ReturnsAsync(datos);

            // Actuar
            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, datos);

            // Verificar
            Assert.NotNull(resultado);
            Assert.Equal("MiAlias", resultado.Alias);
            Assert.Equal("123456789", resultado.NumeroCuenta);
            Assert.Equal("Juan Perez", resultado.TitularCuenta);
            Assert.Equal("1234567890123456789012", resultado.Cbu);
            
            _mockRepositorio.Verify(x => x.ActualizarDatosTransferenciaAsync(restauranteId, datos), Times.Once);
        }
    }
}
