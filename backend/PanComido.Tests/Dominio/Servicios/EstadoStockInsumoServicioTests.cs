using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Servicios;

namespace PanComido.Tests.Dominio.Servicios
{
    public class EstadoStockInsumoServicioTests
    {
        private readonly EstadoStockInsumoServicio _servicio;

        public EstadoStockInsumoServicioTests()
        {
            _servicio = new EstadoStockInsumoServicio();
        }

        [Theory]
        [InlineData(4, 5, EstadoStock.Critico)] // stockActual < stockMinimo
        [InlineData(0, 5, EstadoStock.Critico)] // Caso extremo: Sin stock
        [InlineData(5, 5, EstadoStock.Bajo)]    // Borde exacto: stockActual == stockMinimo
        [InlineData(8, 5, EstadoStock.Bajo)]    // stockActual < stockMinimo * 2
        [InlineData(10, 5, EstadoStock.Normal)] // Borde exacto: stockActual == stockMinimo * 2
        [InlineData(15, 5, EstadoStock.Normal)] // stockActual > stockMinimo * 2
        public void CalcularEstadoStock_DeberiaDevolverEstadoCorrecto(
            decimal stockActual,
            decimal stockMinimo,
            EstadoStock estadoEsperado)
        {
            var resultado = _servicio.CalcularEstadoStock(stockActual, stockMinimo);

            Assert.Equal(estadoEsperado, resultado);
        }
    }
}