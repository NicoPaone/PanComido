using Moq;
using PanComido.Dominio.CasosDeUso.ComandaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Tests.Dominio.CasosDeUso.Comandas
{
    public class LlamarMozoComandaCasoDeUsoTest
    {
        private readonly Mock<IComandaRepositorio> _comandaRepoMock;
        private readonly Mock<IMozoRepositorio> _mozoRepoMock;
        private readonly Mock<IComandaNotificador> _notificadorMock;

        public LlamarMozoComandaCasoDeUsoTest()
        {
            _comandaRepoMock = new Mock<IComandaRepositorio>();
            _mozoRepoMock = new Mock<IMozoRepositorio>();
            _notificadorMock = new Mock<IComandaNotificador>();
        }

        private LlamarMozoComandaCasoDeUso CrearCasoDeUsoConReposMock()
        {
            return new LlamarMozoComandaCasoDeUso(
                _comandaRepoMock.Object,
                _mozoRepoMock.Object,
                _notificadorMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoComandaNoExiste_RetornaSinHacerNada()
        {
            // 1. Preparar
            LlamarMozoComandaCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int restauranteId = 1;
            int comandaId = 99;

            Comanda comandaNula = null;

            _comandaRepoMock.Setup(r => r.ObtenerComandaPorIdAsync(comandaId)).ReturnsAsync(comandaNula);

            // 2. Ejecutar
            await casoDeUso.EjecutarAsync(restauranteId, comandaId);

            // 3. Verificar
            // no llama a mozo ni notificador
            _mozoRepoMock.Verify(r => r.ObtenerMozoAsignadoAMesaAsync(It.IsAny<int>()), Times.Never);
            _notificadorMock.Verify(n => n.NotificarLlamadoCocinaAsync(It.IsAny<Comanda>(), It.IsAny<List<int>>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoComandaEsDeOtroRestaurante_RetornaSinHacerNada()
        {
            // 1. Preparar
            LlamarMozoComandaCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int restauranteId = 1;
            int comandaId = 5;

            // comanda que pertenece al restaurante 2 (intruso)
            Comanda comandaIntrusa = new Comanda { Id = comandaId, RestauranteId = 2 };

            _comandaRepoMock.Setup(r => r.ObtenerComandaPorIdAsync(comandaId)).ReturnsAsync(comandaIntrusa);

            // 2. Ejecutar
            await casoDeUso.EjecutarAsync(restauranteId, comandaId);

            // 3. Verificar
            // no se llama a nada
            _mozoRepoMock.Verify(r => r.ObtenerMozoAsignadoAMesaAsync(It.IsAny<int>()), Times.Never);
            _notificadorMock.Verify(n => n.NotificarLlamadoCocinaAsync(It.IsAny<Comanda>(), It.IsAny<List<int>>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoNoHayMozoAsignado_NoEnviaNotificacion()
        {
            // 1. Preparar
            LlamarMozoComandaCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int restauranteId = 1;
            int comandaId = 5;
            int mesaId = 10;

            Comanda comandaValida = new Comanda { Id = comandaId, RestauranteId = restauranteId, MesaId = mesaId };

            _comandaRepoMock.Setup(r => r.ObtenerComandaPorIdAsync(comandaId)).ReturnsAsync(comandaValida);

            // ningun mzoo asignado
            _mozoRepoMock.Setup(r => r.ObtenerMozoAsignadoAMesaAsync(mesaId)).ReturnsAsync(0);

            // 2. Ejecutar
            await casoDeUso.EjecutarAsync(restauranteId, comandaId);

            // 3. Verificar
            _mozoRepoMock.Verify(r => r.ObtenerMozoAsignadoAMesaAsync(mesaId), Times.Once);
            _notificadorMock.Verify(n => n.NotificarLlamadoCocinaAsync(It.IsAny<Comanda>(), It.IsAny<List<int>>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_CuandoTodoEsValido_EnviaNotificacionAlMozo()
        {
            // 1. Preparar
            LlamarMozoComandaCasoDeUso casoDeUso = CrearCasoDeUsoConReposMock();
            int restauranteId = 1;
            int comandaId = 5;
            int mesaId = 10;
            int mozoId = 15; // Mozo existente

            Comanda comandaValida = new Comanda { Id = comandaId, RestauranteId = restauranteId, MesaId = mesaId };

            _comandaRepoMock.Setup(r => r.ObtenerComandaPorIdAsync(comandaId)).ReturnsAsync(comandaValida);
            _mozoRepoMock.Setup(r => r.ObtenerMozoAsignadoAMesaAsync(mesaId)).ReturnsAsync(mozoId);

            // 2. Ejecutar
            await casoDeUso.EjecutarAsync(restauranteId, comandaId);

            // 3. Verificar
            _notificadorMock.Verify(n => n.NotificarLlamadoCocinaAsync(
                comandaValida,
                It.Is<List<int>>(lista => lista.Count == 1 && lista.Contains(mozoId)) 
            ), Times.Once);
        }
    }
}
