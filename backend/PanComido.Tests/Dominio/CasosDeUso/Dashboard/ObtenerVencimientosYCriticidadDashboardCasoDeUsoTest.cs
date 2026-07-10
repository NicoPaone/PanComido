using Moq;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PanComido.Tests.Dominio.CasosDeUso.Dashboard
{
    public class ObtenerVencimientosYCriticidadDashboardCasoDeUsoTest
    {
        private readonly Mock<IInsumoRepositorio> _insumoRepoMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly ObtenerVencimientosYCriticidadDashboardCasoDeUso _casoDeUso;

        public ObtenerVencimientosYCriticidadDashboardCasoDeUsoTest()
        {
            _insumoRepoMock = new Mock<IInsumoRepositorio>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _casoDeUso = new ObtenerVencimientosYCriticidadDashboardCasoDeUso(_insumoRepoMock.Object, _dateTimeProviderMock.Object);
        }

        [Fact]
        public async Task EjecutarAsync_SoloRetornaInsumosQueVencenEnDiezDiasOMenos()
        {
            int restauranteId = 1;
            var fechaReferencia = new DateTime(2023, 1, 1);
            _dateTimeProviderMock.Setup(d => d.ObtenerAhora()).Returns(fechaReferencia);

            var hoy = DateOnly.FromDateTime(fechaReferencia);
            
            var insumos = new List<Insumo>
            {
                new Insumo { Id = 1, Nombre = "Insumo 1", Vencimiento = hoy.AddDays(1) },
                new Insumo { Id = 2, Nombre = "Insumo 2", Vencimiento = hoy.AddDays(10) },
                new Insumo { Id = 3, Nombre = "Insumo 3", Vencimiento = hoy.AddDays(11) },
                new Insumo { Id = 4, Nombre = "Insumo 4", Vencimiento = null }
            };

            _insumoRepoMock.Setup(r => r.ObtenerInsumosProximosAVencerAsync(restauranteId))
                .ReturnsAsync(insumos);

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId);

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Contains(resultado, i => i.Id == 1);
            Assert.Contains(resultado, i => i.Id == 2);
        }

        [Fact]
        public async Task EjecutarAsync_AsignaCriticidadAltaParaVencimientosMenoresADosDias()
        {
            int restauranteId = 1;
            var fechaReferencia = new DateTime(2023, 1, 1);
            _dateTimeProviderMock.Setup(d => d.ObtenerAhora()).Returns(fechaReferencia);

            var hoy = DateOnly.FromDateTime(fechaReferencia);
            
            var insumos = new List<Insumo>
            {
                new Insumo { Id = 1, Nombre = "Insumo Alta", Vencimiento = hoy.AddDays(1) }, 
                new Insumo { Id = 2, Nombre = "Insumo Media", Vencimiento = hoy.AddDays(4) }, 
                new Insumo { Id = 3, Nombre = "Insumo Baja", Vencimiento = hoy.AddDays(9) }  
            };

            _insumoRepoMock.Setup(r => r.ObtenerInsumosProximosAVencerAsync(restauranteId))
                .ReturnsAsync(insumos);

            var resultado = await _casoDeUso.EjecutarAsync(restauranteId);

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
