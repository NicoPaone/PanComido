using PanComido.Dominio.Servicios;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Tests.Dominio.Servicios
{
    public class CalcularTotalComandaServicioTest
    {
        private readonly CalcularTotalComandaServicio _servicio;

        public CalcularTotalComandaServicioTest()
        {
            _servicio = new CalcularTotalComandaServicio();
        }

        [Fact]
        public void CalcularTotal_CuandoHayVariosItems_DevuelveElTotalCorrecto()
        {
            var comanda = new DOM.Comanda
            {
                Items = new List<DOM.ArticuloComanda>
                {
                    new DOM.ArticuloComanda { Cantidad = 2, Articulo = new DOM.Plato { PrecioVentaFinal = 500 } },
                    new DOM.ArticuloComanda { Cantidad = 3, Articulo = new DOM.Plato { PrecioVentaFinal = 200 } }
                }
            };

            var resultado = _servicio.CalcularTotal(comanda);

            Assert.Equal(1600m, resultado); // 2*500 + 3*200
        }

        [Fact]
        public void CalcularTotal_CuandoElPrecioEsNull_LoCuentaComoZero()
        {
            var comanda = new DOM.Comanda
            {
                Items = new List<DOM.ArticuloComanda>
                {
                    new DOM.ArticuloComanda { Cantidad = 2, Articulo = new DOM.Plato { PrecioVentaFinal = null } },
                    new DOM.ArticuloComanda { Cantidad = 1, Articulo = new DOM.Plato { PrecioVentaFinal = 300 } }
                }
            };

            var resultado = _servicio.CalcularTotal(comanda);

            Assert.Equal(300m, resultado);
        }

        [Fact]
        public void CalcularTotal_CuandoNoHayItems_DevuelveCero()
        {
            var comanda = new DOM.Comanda
            {
                Items = new List<DOM.ArticuloComanda>()
            };

            var resultado = _servicio.CalcularTotal(comanda);

            Assert.Equal(0m, resultado);
        }

        [Fact]
        public void CalcularTotal_CuandoHayUnSoloItem_DevuelveElPrecioMultiplicado()
        {
            var comanda = new DOM.Comanda
            {
                Items = new List<DOM.ArticuloComanda>
                {
                    new DOM.ArticuloComanda { Cantidad = 4, Articulo = new DOM.Plato { PrecioVentaFinal = 150 } }
                }
            };

            var resultado = _servicio.CalcularTotal(comanda);

            Assert.Equal(600m, resultado);
        }
    }
}