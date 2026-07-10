using PanComido.Dominio.Entidades;
using PanComido.Dominio.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Tests.Dominio.Servicios
{
    public class DisponibilidadArticuloServicioTest
    {
        [Fact]
        public void VerificarDisponibilidad_CuandoPlatoTieneStockSuficiente_RetornaTrue()
        {
            // Preparar
            var servicio = new DisponibilidadArticuloServicio();

            var plato = new Plato
            {
                Ingredientes = new List<PlatoIngrediente>
                {
                    new PlatoIngrediente
                    {
                        InsumoId = 1,
                        Cantidad = 2,
                        Opcional = false
                    }
                }
            };

            var stock = new Dictionary<int, decimal>
            {
                { 1, 10 }
            };

            // Ejecutar
            var resultado = servicio.VerificarDisponibilidad(plato, 3, stock);

            // Verificar
            Assert.True(resultado);
        }

        [Fact]
        public void VerificarDisponibilidad_CuandoPlatoNoTieneStockSuficiente_RetornaFalse()
        {
            // Preparar
            var servicio = new DisponibilidadArticuloServicio();

            var plato = new Plato
            {
                Ingredientes = new List<PlatoIngrediente>
                {
                    new PlatoIngrediente
                    {
                        InsumoId = 1,
                        Cantidad = 2,
                        Opcional = false
                    }
                }
            };

            var stock = new Dictionary<int, decimal>
            {
                { 1, 5 }
            };

            // Ejecutar
            var resultado = servicio.VerificarDisponibilidad(plato, 3, stock);

            // Verificar
            Assert.False(resultado);
        }

        [Fact]
        public void VerificarDisponibilidad_CuandoIngredienteEsOpcional_YNoHayStock_RetornaTrue()
        {
            // Preparar
            var servicio = new DisponibilidadArticuloServicio();

            var plato = new Plato
            {
                Ingredientes = new List<PlatoIngrediente>
                {
                    new PlatoIngrediente
                    {
                        InsumoId = 1,
                        Cantidad = 2,
                        Opcional = true
                    }
                }
            };

            var stock = new Dictionary<int, decimal>();

            // Ejecutar
            var resultado = servicio.VerificarDisponibilidad(plato, 1, stock);

            // Verificar
            Assert.True(resultado);
        }

        [Fact]
        public void VerificarDisponibilidad_CuandoBebidaTieneStockSuficiente_RetornaTrue()
        {
            // Preparar
            var servicio = new DisponibilidadArticuloServicio();

            var bebida = new Insumo
            {
                Id = 1
            };

            var stock = new Dictionary<int, decimal>
            {
                { 1, 10 }
            };

            // Ejecutar
            var resultado = servicio.VerificarDisponibilidad(bebida, 5, stock);

            // Verificar
            Assert.True(resultado);
        }

        [Fact]
        public void VerificarDisponibilidad_CuandoBebidaNoTieneStockSuficiente_RetornaFalse()
        {
            // Preparar
            var servicio = new DisponibilidadArticuloServicio();

            var bebida = new Insumo
            {
                Id = 1
            };

            var stock = new Dictionary<int, decimal>
            {
                { 1, 2 }
            };

            // Ejecutar
            var resultado = servicio.VerificarDisponibilidad(bebida, 5, stock);

            // Verificar
            Assert.False(resultado);
        }

        [Fact]
        public void VerificarDisponibilidad_CuandoBebidaPreparadaTieneStockSuficiente_RetornaTrue()
        {
            // Preparar
            var servicio = new DisponibilidadArticuloServicio();

            var bebidaPreparada = new BebidaPreparada
            {
                Insumos = new List<BebidaPreparadaInsumo>
                {
                    new BebidaPreparadaInsumo { InsumoId = 1, Cantidad = 100 },
                    new BebidaPreparadaInsumo { InsumoId = 2, Cantidad = 300 }
                }
            };

            var stock = new Dictionary<int, decimal>
            {
                { 1, 1000 },
                { 2, 2000 }
            };

            // Ejecutar
            var resultado = servicio.VerificarDisponibilidad(bebidaPreparada, 2, stock);

            // Verificar
            Assert.True(resultado);
        }

        [Fact]
        public void VerificarDisponibilidad_CuandoBebidaPreparadaNoTieneStockSuficiente_RetornaFalse()
        {
            // Preparar
            var servicio = new DisponibilidadArticuloServicio();

            var bebidaPreparada = new BebidaPreparada
            {
                Insumos = new List<BebidaPreparadaInsumo>
                {
                    new BebidaPreparadaInsumo { InsumoId = 1, Cantidad = 100 },
                    new BebidaPreparadaInsumo { InsumoId = 2, Cantidad = 300 }
                }
            };

            var stock = new Dictionary<int, decimal>
            {
                { 1, 1000 },
                { 2, 100 }
            };

            // Ejecutar
            var resultado = servicio.VerificarDisponibilidad(bebidaPreparada, 2, stock);

            // Verificar
            Assert.False(resultado);
        }

        [Fact]
        public void VerificarDisponibilidad_SobrecargaSinCantidad_UsaCantidadUno()
        {
            // Preparar
            var servicio = new DisponibilidadArticuloServicio();

            var bebida = new Insumo
            {
                Id = 1
            };

            var stock = new Dictionary<int, decimal>
            {
                { 1, 1 }
            };

            // Ejecutar
            var resultado = servicio.VerificarDisponibilidad(bebida, stock);

            // Verificar
            Assert.True(resultado);
        }
    }
}