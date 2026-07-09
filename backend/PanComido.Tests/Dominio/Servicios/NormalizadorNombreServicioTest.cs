using PanComido.Dominio.Servicios;

namespace PanComido.Tests.Dominio.Servicios
{
    public class NormalizadorNombreServicioTest
    {
        private readonly NormalizadorNombreServicio _servicio = new NormalizadorNombreServicio();

        [Theory]
        [InlineData("harina", "Harina")]
        [InlineData("HARINA BLANCA", "Harina blanca")]
        [InlineData("  coca cola  ", "Coca cola")]
        [InlineData("aceite de oliva", "Aceite de oliva")]
        public void Normalizar_CapitalizaSoloLaPrimeraLetraYRecortaEspacios(string entrada, string esperado)
        {
            string resultado = _servicio.Normalizar(entrada);

            Assert.Equal(esperado, resultado);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Normalizar_CuandoEsNuloOVacio_DevuelveElValorOriginal(string entrada)
        {
            string resultado = _servicio.Normalizar(entrada);

            Assert.Equal(entrada, resultado);
        }
    }
}
