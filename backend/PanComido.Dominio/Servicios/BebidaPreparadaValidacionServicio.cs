using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.Servicios
{
    public class BebidaPreparadaValidacionServicio : IBebidaPreparadaValidacionServicio
    {
        private readonly ILogger<BebidaPreparadaValidacionServicio> _logger;

        public BebidaPreparadaValidacionServicio(ILogger<BebidaPreparadaValidacionServicio> logger)
        {
            _logger = logger;
        }

        public void ValidarDatosBasicos(BebidaPreparada bebidaPreparada)
        {
            if (string.IsNullOrWhiteSpace(bebidaPreparada.Nombre))
            {
                _logger.LogWarning("Rechazo de validación: el nombre de la bebida preparada no puede estar vacío.");
                throw new ArgumentException("El nombre de la bebida preparada no puede estar vacío.");
            }

            if (bebidaPreparada.PrecioVentaFinal <= 0)
            {
                _logger.LogWarning("Rechazo de validación: el precio de venta final de '{Nombre}' debe ser mayor que cero.", bebidaPreparada.Nombre);
                throw new ArgumentException("El precio de venta final debe ser mayor que cero.");
            }

            if (bebidaPreparada.Insumos == null || !bebidaPreparada.Insumos.Any())
            {
                _logger.LogWarning("Rechazo de validación: la bebida preparada '{Nombre}' debe tener al menos un insumo en su receta.", bebidaPreparada.Nombre);
                throw new ArgumentException("La bebida preparada debe tener al menos un insumo en su receta.");
            }
        }
    }
}
