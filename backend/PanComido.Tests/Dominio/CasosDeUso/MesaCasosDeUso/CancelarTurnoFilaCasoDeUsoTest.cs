using Moq;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class CancelarTurnoFilaCasoDeUsoTest
    {
        private readonly Mock<ITurnoFilaRepositorio> _mockRepositorio;
        private readonly CancelarTurnoFilaCasoDeUso _casoDeUso;

        public CancelarTurnoFilaCasoDeUsoTest()
        {
            _mockRepositorio = new Mock<ITurnoFilaRepositorio>();
            _casoDeUso = new CancelarTurnoFilaCasoDeUso(_mockRepositorio.Object);
        }

        [Fact]
        public async Task EjecutarAsync_TurnoNoExiste_LanzaArgumentException()
        {
            // Preparar
            int turnoId = 99;
            _mockRepositorio.Setup(x => x.ObtenerPorIdAsync(turnoId))
                .ReturnsAsync((TurnoFila?)null);

            // Actuar & Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() => 
                _casoDeUso.EjecutarAsync(turnoId));

            Assert.Equal("Turno de fila no encontrado", excepcion.Message);
        }

        [Theory]
        [InlineData(EstadoTurnoMesa.Completado)]
        [InlineData(EstadoTurnoMesa.Cancelado)]
        public async Task EjecutarAsync_TurnoYaCompletadoOCancelado_LanzaInvalidOperationException(EstadoTurnoMesa estadoInvalido)
        {
            // Preparar
            int turnoId = 1;
            var turno = new TurnoFila { Id = turnoId, Estado = estadoInvalido };
            
            _mockRepositorio.Setup(x => x.ObtenerPorIdAsync(turnoId))
                .ReturnsAsync(turno);

            // Actuar & Verificar
            var excepcion = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _casoDeUso.EjecutarAsync(turnoId));

            Assert.Equal("El turno ya está completado o cancelado.", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_TurnoValido_ActualizaEstadoACancelado()
        {
            // Preparar
            int turnoId = 1;
            var turno = new TurnoFila { Id = turnoId, Estado = EstadoTurnoMesa.EnEspera };
            
            _mockRepositorio.Setup(x => x.ObtenerPorIdAsync(turnoId))
                .ReturnsAsync(turno);

            // Actuar
            await _casoDeUso.EjecutarAsync(turnoId);

            // Verificar
            Assert.Equal(EstadoTurnoMesa.Cancelado, turno.Estado);
            _mockRepositorio.Verify(x => x.ActualizarAsync(turno), Times.Once);
        }
    }
}
