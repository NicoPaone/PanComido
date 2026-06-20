using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso
{
    public class ActualizarMetodosDePagoCasoDeUso
    {
        private readonly IMetodoDePagoRepositorio _metodoDePagoRepositorio;
        private readonly ILogger<ActualizarMetodosDePagoCasoDeUso> _logger;

        public ActualizarMetodosDePagoCasoDeUso(IMetodoDePagoRepositorio metodoDePagoRepositorio, ILogger<ActualizarMetodosDePagoCasoDeUso> logger)
        {
            _metodoDePagoRepositorio = metodoDePagoRepositorio;
            _logger = logger;
        }

        public async Task EjecutarAsync(int restauranteId, List<MetodoDePago> metodosDePago)
        {
            await _metodoDePagoRepositorio.ActualizarEstadoAsync(restauranteId, metodosDePago);
            _logger.LogInformation("Métodos de pago actualizados. RestauranteId: {RestauranteId}", restauranteId);
        }
    }
}