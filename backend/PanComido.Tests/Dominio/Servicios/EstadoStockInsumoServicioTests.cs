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
        [InlineData(4, 5, 10, EstadoStock.Critico)] // stockActual < stockMinimo
        [InlineData(0, 5, 10, EstadoStock.Critico)] // Caso extremo: Sin stock
        [InlineData(5, 5, 10, EstadoStock.Bajo)]    // Borde exacto: stockActual == stockMinimo
        [InlineData(8, 5, 10, EstadoStock.Bajo)]    // stockActual < stockRecomendado
        [InlineData(10, 5, 10, EstadoStock.Normal)] // Borde exacto: stockActual == stockRecomendado
        [InlineData(15, 5, 10, EstadoStock.Normal)] // stockActual > stockRecomendado
        public void CalcularEstadoStock_DeberiaDevolverEstadoCorrecto(
            decimal stockActual,
            decimal stockMinimo,
            decimal stockRecomendado,
            EstadoStock estadoEsperado)
        {
            var resultado = _servicio.CalcularEstadoStock(stockActual, stockMinimo, stockRecomendado);

            Assert.Equal(estadoEsperado, resultado);
        }
    }
}