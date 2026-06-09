using PanComido.Dominio.Entidades;
using PanComido.Dominio.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Tests.Dominio.Servicios
{
    public class VencimientosProximosInsumosServicioTest
    {
        [Fact]
        public void ObtenerVencimientosProximos_DevuelveLotesQueVencenDentroDelPlazo()
        {
            // Preparar
            var servicio = new VencimientosProximosInsumosServicio();

            var insumos = new List<Insumo>
            {
                new Insumo
                {
                    Id = 1,
                    Lotes = new List<Lote>
                    {
                        new Lote
                        {
                            Id = 1,
                            FechaVencimiento = DateOnly.FromDateTime(
                                DateTime.Today.AddDays(3))
                        },
                        new Lote
                        {
                            Id = 2,
                            FechaVencimiento = DateOnly.FromDateTime(
                                DateTime.Today.AddDays(20))
                        }
                    }
                }
            };

            // Ejecutar
            var resultado = servicio.ObtenerVencimientosProximos(insumos, 7);

            // Verificar
            Assert.Single(resultado);
            Assert.True(resultado.ContainsKey(1));

            Assert.Single(resultado[1]);
            Assert.Equal(1, resultado[1][0].Id);
        }

        [Fact]
        public void ObtenerVencimientosProximos_DevuelveVacio_CuandoNoHayLotesProximos()
        {
            // Preparar
            var servicio = new VencimientosProximosInsumosServicio();

            var insumos = new List<Insumo>
            {
                new Insumo
                {
                    Id = 1,
                    Lotes = new List<Lote>
                    {
                        new Lote
                        {
                            Id = 1,
                            FechaVencimiento = DateOnly.FromDateTime(
                                DateTime.Today.AddDays(30))
                        }
                    }
                }
            };

            // Ejecutar
            var resultado = servicio.ObtenerVencimientosProximos(insumos, 7);

            // Verificar
            Assert.Empty(resultado);
        }

        [Fact]
        public void ObtenerVencimientosProximos_DevuelveTodosLosLotesProximosDelInsumo()
        {
            // Preparar
            var servicio = new VencimientosProximosInsumosServicio();

            var insumos = new List<Insumo>
            {
                new Insumo
                {
                    Id = 1,
                    Lotes = new List<Lote>
                    {
                        new Lote
                        {
                            Id = 1,
                            FechaVencimiento = DateOnly.FromDateTime(
                                DateTime.Today.AddDays(2))
                        },
                        new Lote
                        {
                            Id = 2,
                            FechaVencimiento = DateOnly.FromDateTime(
                                DateTime.Today.AddDays(5))
                        },
                        new Lote
                        {
                            Id = 3,
                            FechaVencimiento = DateOnly.FromDateTime(
                                DateTime.Today.AddDays(15))
                        }
                    }
                }
            };

            // Ejecutar
            var resultado = servicio.ObtenerVencimientosProximos(insumos, 7);

            // Verificar
            Assert.True(resultado.ContainsKey(1));

            Assert.Equal(2, resultado[1].Count);

            Assert.Contains(resultado[1], l => l.Id == 1);
            Assert.Contains(resultado[1], l => l.Id == 2);
        }

        [Fact]
        public void ObtenerVencimientosProximos_RespetaLosDiasDeAnticipacion()
        {
            // Preparar
            var servicio = new VencimientosProximosInsumosServicio();

            var insumos = new List<Insumo>
            {
                new Insumo
                {
                    Id = 1,
                    Lotes = new List<Lote>
                    {
                        new Lote
                        {
                            Id = 1,
                            FechaVencimiento = DateOnly.FromDateTime(
                                DateTime.Today.AddDays(10))
                        }
                    }
                }
            };

            // Ejecutar
            var resultado = servicio.ObtenerVencimientosProximos(insumos, 15);

            // Verificar
            Assert.Single(resultado);
            Assert.True(resultado.ContainsKey(1));
            Assert.Single(resultado[1]);
            Assert.Equal(1, resultado[1][0].Id);
        }
    }
}