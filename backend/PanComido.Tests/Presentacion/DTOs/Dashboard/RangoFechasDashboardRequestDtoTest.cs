using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PanComido.Presentacion.DTOs.Dashboard;

namespace PanComido.Tests.Presentacion.DTOs.Dashboard
{
    public class RangoFechasDashboardRequestDtoTest
    {
        [Fact]
        public void Validate_CuandoDesdeEsPosteriorAHasta_RetornaError()
        {
            var dto = new RangoFechasDashboardRequestDto
            {
                Desde = new DateTime(2026, 7, 10),
                Hasta = new DateTime(2026, 7, 9)
            };

            var errores = dto.Validate(new ValidationContext(dto)).ToList();

            Assert.Single(errores);
            Assert.Contains("fecha de inicio", errores[0].ErrorMessage);
            Assert.Contains(nameof(RangoFechasDashboardRequestDto.Desde), errores[0].MemberNames);
            Assert.Contains(nameof(RangoFechasDashboardRequestDto.Hasta), errores[0].MemberNames);
        }

        [Fact]
        public void Validate_CuandoRangoEsValido_NoRetornaErrores()
        {
            var dto = new RangoFechasDashboardRequestDto
            {
                Desde = new DateTime(2026, 7, 9),
                Hasta = new DateTime(2026, 7, 10)
            };

            var errores = dto.Validate(new ValidationContext(dto)).ToList();

            Assert.Empty(errores);
        }

        [Fact]
        public void TryValidateObject_CuandoFaltanFechas_RetornaErroresRequeridos()
        {
            var dto = new RangoFechasDashboardRequestDto();
            var errores = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(dto, new ValidationContext(dto), errores, validateAllProperties: true);

            Assert.False(esValido);
            Assert.Contains(errores, error => error.MemberNames.Contains(nameof(RangoFechasDashboardRequestDto.Desde)));
            Assert.Contains(errores, error => error.MemberNames.Contains(nameof(RangoFechasDashboardRequestDto.Hasta)));
        }
    }
}
