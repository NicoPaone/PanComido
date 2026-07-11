using Moq;
using PanComido.Dominio.CasosDeUso.LoteCasosDeUso;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Lotes
{
    public class CrearLoteCasoDeUsoTest
    {
        private readonly Mock<ILoteRepositorio> _mockLoteRepositorio;
        private readonly Mock<IInsumoRepositorio> _mockInsumoRepositorio;
        private readonly Mock<IGeneradorNombreLoteServicio> _mockGeneradorNombreLote;
        private readonly CrearLoteCasoDeUso _casoDeUso;

        public CrearLoteCasoDeUsoTest()
        {
            _mockLoteRepositorio = new Mock<ILoteRepositorio>();
            _mockInsumoRepositorio = new Mock<IInsumoRepositorio>();
            _mockGeneradorNombreLote = new Mock<IGeneradorNombreLoteServicio>();

            _casoDeUso = new CrearLoteCasoDeUso(
                _mockLoteRepositorio.Object,
                _mockInsumoRepositorio.Object,
                _mockGeneradorNombreLote.Object);
        }

        [Fact]
        public async Task EjecutarAsync_InsumoNoExiste_LanzaExcepcion()
        {
            // Preparar
            int restauranteId = 1;
            int insumoId = 99;

            _mockInsumoRepositorio.Setup(x => x.ObtenerPorIdAsync(insumoId, restauranteId))
                .ReturnsAsync((Insumo?)null);

            // Actuar & Verificar
            var excepcion = await Assert.ThrowsAsync<Exception>(() => 
                _casoDeUso.EjecutarAsync(restauranteId, insumoId, 10m, DateOnly.FromDateTime(DateTime.Now.AddDays(10)), 1));

            Assert.Equal("El insumo especificado no existe.", excepcion.Message);
        }

        [Fact]
        public async Task EjecutarAsync_InsumoExiste_CreaLoteExitosamente()
        {
            // Preparar
            int restauranteId = 1;
            int insumoId = 5;
            int bodegaId = 2;
            decimal cantidad = 15.5m;
            var fechaVencimiento = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            var insumo = new Insumo { Id = insumoId, Nombre = "Harina" };

            _mockInsumoRepositorio.Setup(x => x.ObtenerPorIdAsync(insumoId, restauranteId))
                .ReturnsAsync(insumo);

            _mockGeneradorNombreLote.Setup(x => x.GenerarNombreUnicoAsync("Harina"))
                .ReturnsAsync("LOTE-HARINA-123");

            // Actuar
            var resultado = await _casoDeUso.EjecutarAsync(restauranteId, insumoId, cantidad, fechaVencimiento, bodegaId);

            // Verificar
            Assert.Equal(1, resultado);
            
            _mockLoteRepositorio.Verify(x => x.CrearLotesAsync(It.Is<List<Lote>>(lotes => 
                lotes.Count == 1 &&
                lotes[0].Nombre == "LOTE-HARINA-123" &&
                lotes[0].InsumoId == insumoId &&
                lotes[0].Cantidad == cantidad &&
                lotes[0].BodegaId == bodegaId &&
                lotes[0].FechaVencimiento == fechaVencimiento
            )), Times.Once);
        }
    }
}
