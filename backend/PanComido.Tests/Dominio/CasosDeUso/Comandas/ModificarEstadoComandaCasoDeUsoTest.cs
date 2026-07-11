using Moq;
using PanComido.Dominio.CasosDeUso.ComandaCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Comandas
{
    public class ModificarEstadoComandaCasoDeUsoTest
    {
        private readonly Mock<IComandaRepositorio> _mockComandaRepositorio;
        private readonly Mock<IComandaNotificador> _mockComandaNotificador;
        private readonly Mock<IMesaRepositorio> _mockMesaRepositorio;
        private readonly ModificarEstadoComandaCasoDeUso _casoDeUso;

        public ModificarEstadoComandaCasoDeUsoTest()
        {
            _mockComandaRepositorio = new Mock<IComandaRepositorio>();
            _mockComandaNotificador = new Mock<IComandaNotificador>();
            _mockMesaRepositorio = new Mock<IMesaRepositorio>();

            _casoDeUso = new ModificarEstadoComandaCasoDeUso(
                _mockComandaRepositorio.Object,
                _mockComandaNotificador.Object,
                _mockMesaRepositorio.Object);
        }

        [Theory]
        [InlineData(EstadoComanda.Finalizada)]
        [InlineData(EstadoComanda.Abierta)]
        public async Task EjecutarAsync_EstadoInicialInvalido_LanzaArgumentException(EstadoComanda estadoActual)
        {
            // Preparar
            int comandaId = 1;
            var comanda = new Comanda { Id = comandaId, Estado = estadoActual };

            _mockComandaRepositorio.Setup(x => x.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(comanda);

            // Actuar & Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() => 
                _casoDeUso.EjecutarAsync(comandaId, (int)EstadoComanda.EnPreparacion));

            Assert.Equal("La comanda se encuentra en un estado que no puede ser cambiado desde esta acción", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_PasaAEnPreparacionPeroNoEsNueva_LanzaArgumentException()
        {
            // Preparar
            int comandaId = 1;
            var comanda = new Comanda { Id = comandaId, Estado = EstadoComanda.EnEspera };

            _mockComandaRepositorio.Setup(x => x.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(comanda);

            // Actuar & Verificar
            var excepcion = await Assert.ThrowsAsync<ArgumentException>(() => 
                _casoDeUso.EjecutarAsync(comandaId, (int)EstadoComanda.EnPreparacion));

            Assert.Equal("La comanda se encuentra en un estado en el que no puede pasar a 'En preparación'", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_ComandaNoEncontradaDespuesDeModificar_LanzaKeyNotFoundException()
        {
            // Preparar
            int comandaId = 1;
            var comanda = new Comanda { Id = comandaId, Estado = EstadoComanda.Nueva };

            int callCount = 0;
            _mockComandaRepositorio.Setup(x => x.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(() => callCount++ == 0 ? comanda : null);

            _mockComandaRepositorio.Setup(x => x.ModificarEstadoComandaAsync(comandaId, (int)EstadoComanda.EnPreparacion))
                .ReturnsAsync(new Comanda { Id = comandaId });

            // Actuar & Verificar
            var excepcion = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _casoDeUso.EjecutarAsync(comandaId, (int)EstadoComanda.EnPreparacion));

            Assert.Equal("No se encontró una comanda activa para esa mesa.", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_CaminoFeliz_ModificaYRetornaComandaConPlatos()
        {
            // Preparar
            int comandaId = 1;
            int mesaId = 5;
            var comandaOriginal = new Comanda { Id = comandaId, Estado = EstadoComanda.Nueva, MesaId = mesaId };
            
            var itemPlato = new ArticuloComanda { Id = 1, Articulo = new Plato { Nombre = "Milanesa" } };
            var itemBebida = new ArticuloComanda { Id = 2, Articulo = new PanComido.Dominio.Entidades.BebidaPreparada { Nombre = "Gaseosa" } };

            var comandaModificada = new Comanda 
            { 
                Id = comandaId, 
                Estado = EstadoComanda.EnPreparacion, 
                MesaId = mesaId,
                Items = new List<ArticuloComanda> { itemPlato, itemBebida }
            };

            int callCount = 0;
            _mockComandaRepositorio.Setup(x => x.ObtenerComandaPorIdAsync(comandaId))
                .ReturnsAsync(() => callCount++ == 0 ? comandaOriginal : comandaModificada);

            _mockComandaRepositorio.Setup(x => x.ModificarEstadoComandaAsync(comandaId, (int)EstadoComanda.EnPreparacion))
                .ReturnsAsync(new Comanda { Id = comandaId });

            var mozos = new List<int> { 10, 11 };
            _mockMesaRepositorio.Setup(x => x.ObtenerMozoIdsPorMesaAsync(mesaId))
                .ReturnsAsync(mozos);

            // Actuar
            var resultado = await _casoDeUso.EjecutarAsync(comandaId, (int)EstadoComanda.EnPreparacion);

            // Verificar
            Assert.NotNull(resultado);
            Assert.Equal(EstadoComanda.EnPreparacion, resultado.Estado);
            Assert.Single(resultado.Items); // Solo debe incluir el item que es Plato
            Assert.IsType<Plato>(resultado.Items[0].Articulo);
            
            _mockComandaNotificador.Verify(x => x.NotificarEstadoModificadoAsync(comandaModificada, mozos), Times.Once);
        }
    }
}
