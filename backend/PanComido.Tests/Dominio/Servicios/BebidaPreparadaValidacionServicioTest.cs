using Microsoft.Extensions.Logging;
using Moq;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Servicios;

namespace PanComido.Tests.Dominio.Servicios
{
    public class BebidaPreparadaValidacionServicioTest
    {
        private readonly BebidaPreparadaValidacionServicio _servicio;

        public BebidaPreparadaValidacionServicioTest()
        {
            _servicio = new BebidaPreparadaValidacionServicio(Mock.Of<ILogger<BebidaPreparadaValidacionServicio>>());
        }

        [Fact]
        public void ValidarDatosBasicos_CuandoNombreEsVacio_LanzaArgumentException()
        {
            var bebida = new BebidaPreparada { Nombre = "", PrecioVentaFinal = 100, Insumos = new List<BebidaPreparadaInsumo> { new BebidaPreparadaInsumo() } };

            ArgumentException excepcion = Assert.Throws<ArgumentException>(() => _servicio.ValidarDatosBasicos(bebida));

            Assert.Equal("El nombre de la bebida preparada no puede estar vacío.", excepcion.Message);
        }

        [Fact]
        public void ValidarDatosBasicos_CuandoPrecioEsCero_LanzaArgumentException()
        {
            var bebida = new BebidaPreparada { Nombre = "Fernet con Coca", PrecioVentaFinal = 0, Insumos = new List<BebidaPreparadaInsumo> { new BebidaPreparadaInsumo() } };

            ArgumentException excepcion = Assert.Throws<ArgumentException>(() => _servicio.ValidarDatosBasicos(bebida));

            Assert.Equal("El precio de venta final debe ser mayor que cero.", excepcion.Message);
        }

        [Fact]
        public void ValidarDatosBasicos_CuandoNoTieneInsumos_LanzaArgumentException()
        {
            var bebida = new BebidaPreparada { Nombre = "Fernet con Coca", PrecioVentaFinal = 100, Insumos = new List<BebidaPreparadaInsumo>() };

            ArgumentException excepcion = Assert.Throws<ArgumentException>(() => _servicio.ValidarDatosBasicos(bebida));

            Assert.Equal("La bebida preparada debe tener al menos un insumo en su receta.", excepcion.Message);
        }

        [Fact]
        public void ValidarDatosBasicos_CuandoTodoEsValido_NoLanzaExcepcion()
        {
            var bebida = new BebidaPreparada
            {
                Nombre = "Fernet con Coca",
                PrecioVentaFinal = 100,
                Insumos = new List<BebidaPreparadaInsumo> { new BebidaPreparadaInsumo { InsumoId = 1, Cantidad = 100 } }
            };

            _servicio.ValidarDatosBasicos(bebida);
        }
    }
}
