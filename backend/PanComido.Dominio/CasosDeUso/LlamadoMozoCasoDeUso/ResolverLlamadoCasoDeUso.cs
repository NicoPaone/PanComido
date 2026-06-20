using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.LlamadoMozoCasoDeUso
{
    public class ResolverLlamadoCasoDeUso
    {
        private readonly ILlamadoRepositorio _llamadoRepositorio;
        private readonly ILogger<ResolverLlamadoCasoDeUso> _logger;

        public ResolverLlamadoCasoDeUso(ILlamadoRepositorio llamadoRepositorio, ILogger<ResolverLlamadoCasoDeUso> logger)
        {
            _llamadoRepositorio = llamadoRepositorio;
            _logger = logger;
        }

        public async Task EjecutarAsync(int llamadoId)
        {
            bool respuesta = await _llamadoRepositorio.ResolverLlamadoAsync(llamadoId);
            if (!respuesta)
            {
                _logger.LogWarning("Intento de resolver llamado inexistente. LlamadoId: {LlamadoId}", llamadoId);
                throw new KeyNotFoundException("El llamado no existe.");
            }

            _logger.LogInformation("Llamado resuelto. LlamadoId: {LlamadoId}", llamadoId);
        }
    }
}
