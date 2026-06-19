using Moq;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Dashboard
{
    public class ObtenerVencimientosYCriticidadDashboardCasoDeUsoTest
    {
        private readonly Mock<IInsumoRepositorio> _insumoRepoMock;
        private readonly ObtenerVencimientosYCriticidadDashboardCasoDeUso _casoDeUso;

        public ObtenerVencimientosYCriticidadDashboardCasoDeUsoTest()
        {
            _insumoRepoMock = new Mock<IInsumoRepositorio>();
            _casoDeUso = new ObtenerVencimientosYCriticidadDashboardCasoDeUso(_insumoRepoMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_SoloRetornaInsumosQueVencenEnSieteDiasOMenos()
        {
            // Preparar
            int restauranteId = 1;
            var hoy = DateOnly.FromDateTime(DateTime.Now);
            
            var insumos = new List<Insumo>
            {
                new Insumo { Id = 1, Nombre = "Insumo 1", Vencimiento = hoy.AddDays(1) }, // Entra (1 dia)
                new Insumo { Id = 2, Nombre = "Insumo 2", Vencimiento = hoy.AddDays(7) }, // Entra (7 dias)
                new Insumo { Id = 3, Nombre = "Insumo 3", Vencimiento = hoy.AddDays(8) }, // No entra (8 dias)
                new Insumo { Id = 4, Nombre = "Insumo 4", Vencimiento = null }            // No entra (sin vencimiento)
            };

            _insumoRepoMock.Setup(r => r.ObtenerInsumosProximosAVencerAsync(restauranteId))
                .ReturnsAsync(insumos);

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(restauranteId);

            // Verificar
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Contains(resultado, i => i.Id == 1);
            Assert.Contains(resultado, i => i.Id == 2);
        }

        [Fact]
        public async Task EjecutarAsync_AsignaCriticidadAltaParaVencimientosMenoresADosDias()
        {
            // Preparar
            int restauranteId = 1;
            var hoy = DateOnly.FromDateTime(DateTime.Now);
            
            var insumos = new List<Insumo>
            {
                new Insumo { Id = 1, Nombre = "Insumo Alta", Vencimiento = hoy.AddDays(1) }, 
                new Insumo { Id = 2, Nombre = "Insumo Media", Vencimiento = hoy.AddDays(4) }, 
                new Insumo { Id = 3, Nombre = "Insumo Baja", Vencimiento = hoy.AddDays(6) }  
            };

            _insumoRepoMock.Setup(r => r.ObtenerInsumosProximosAVencerAsync(restauranteId))
                .ReturnsAsync(insumos);

            // Ejecutar
            var resultado = await _casoDeUso.EjecutarAsync(restauranteId);

            // Verificar
            Assert.Equal(3, resultado.Count);
            
            var insumoAlta = resultado.Find(i => i.Id == 1);
            Assert.Equal(CriticidadVencimiento.Alta, insumoAlta.CriticidadVencimiento);

            var insumoMedia = resultado.Find(i => i.Id == 2);
            Assert.Equal(CriticidadVencimiento.Media, insumoMedia.CriticidadVencimiento);

            var insumoBaja = resultado.Find(i => i.Id == 3);
            Assert.Equal(CriticidadVencimiento.Baja, insumoBaja.CriticidadVencimiento);
        }
    }
}
