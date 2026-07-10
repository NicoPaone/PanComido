using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.CasosDeUso.ComandaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Tests.Dominio.CasosDeUso.Comandas
{
    public class ObtenerDatosInvitadoBienvenidaAComandaCasoDeUsoTest
    {
        private readonly Mock<IComandaRepositorio> _comandaRepoMock;
        private readonly Mock<IRestauranteRepositorio> _restauranteRepoMock;
        private readonly Mock<IMesaRepositorio> _mesaRepoMock;
        private readonly Mock<ILogger<ObtenerDatosInvitadoBienvenidaAComandaCasoDeUso>> _loggerMock;

        public ObtenerDatosInvitadoBienvenidaAComandaCasoDeUsoTest()
        {
            _comandaRepoMock = new Mock<IComandaRepositorio>();
            _restauranteRepoMock = new Mock<IRestauranteRepositorio>();
            _mesaRepoMock = new Mock<IMesaRepositorio>();
            _loggerMock = new Mock<ILogger<ObtenerDatosInvitadoBienvenidaAComandaCasoDeUso>>();
        }

        private ObtenerDatosInvitadoBienvenidaAComandaCasoDeUso CrearCasoDeUsoConReposMock()
        {
            return new ObtenerDatosInvitadoBienvenidaAComandaCasoDeUso(
                _comandaRepoMock.Object,
                _restauranteRepoMock.Object,
                _mesaRepoMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoComandaNoExiste_LanzaKeyNotFoundException()
        {
            // 1. Preparar
            ObtenerDatosInvitadoBienvenidaAComandaCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int comandaId = 99;

            Comanda comandaNula = null;

            _comandaRepoMock.Setup(r => r.ObtenerComandaPorIdAsync(comandaId)).ReturnsAsync(comandaNula);

            // 2. Ejecutar y 3. Verificar
            KeyNotFoundException excepcion = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                casoDeUso.EjecutarAsync(comandaId));

            Assert.Equal("La comanda de invitación no existe.", excepcion.Message);

            _restauranteRepoMock.Verify(r => r.ObtenerDatosDelLocalAsync(It.IsAny<int>()), Times.Never);
            _mesaRepoMock.Verify(r => r.ObtenerPorIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoComandaEstaFinalizada_LanzaInvalidOperationException()
        {
            // 1. Preparar
            ObtenerDatosInvitadoBienvenidaAComandaCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int comandaId = 1;

            Comanda comandaFinalizada = new Comanda { Id = comandaId, Estado = EstadoComanda.Finalizada };

            _comandaRepoMock.Setup(r => r.ObtenerComandaPorIdAsync(comandaId)).ReturnsAsync(comandaFinalizada);

            // 2. Ejecutar y 3. Verificar
            InvalidOperationException excepcion = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                casoDeUso.EjecutarAsync(comandaId));

            Assert.Equal("Esta mesa ya ha finalizado su pedido.", excepcion.Message);

            _restauranteRepoMock.Verify(r => r.ObtenerDatosDelLocalAsync(It.IsAny<int>()), Times.Never);
            _mesaRepoMock.Verify(r => r.ObtenerPorIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoComandaEsValida_RetornaDatosDeBienvenida()
        {
            // 1. Preparar
            ObtenerDatosInvitadoBienvenidaAComandaCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int comandaId = 1;
            int restauranteId = 10;
            int mesaId = 5;

            Comanda comandaAbierta = new Comanda
            {
                Id = comandaId,
                Estado = EstadoComanda.Abierta,
                RestauranteId = restauranteId,
                MesaId = mesaId,
                CantComensales = 4
            };

            MesaConPosiciones mesa = new MesaConPosiciones { Id = mesaId, Numero = 12 };

            _comandaRepoMock.Setup(r => r.ObtenerComandaPorIdAsync(comandaId)).ReturnsAsync(comandaAbierta);
            _mesaRepoMock.Setup(r => r.ObtenerPorIdAsync(mesaId, restauranteId)).ReturnsAsync(mesa);

            // 2. Ejecutar
            BienvenidaDatosInvitadoComanda resultado = await casoDeUso.EjecutarAsync(comandaId);

            // 3. Verificar
            Assert.NotNull(resultado);
            Assert.Equal(comandaId, resultado.IdComanda);
            Assert.Equal(4, resultado.CantComensales);
            Assert.Equal(restauranteId, resultado.RestauranteId);

            Assert.NotNull(resultado.Mesa);
            Assert.Equal(12, resultado.Mesa.Numero);

            // se llama a los repos 1 vez
            _mesaRepoMock.Verify(r => r.ObtenerPorIdAsync(mesaId, restauranteId), Times.Once);
        }
    }
}
