using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ReglaTiempoExtraCasosDeUso
{
    public class CrearReglaTiempoExtraCasoDeUso
    {
        private readonly IReglaTiempoExtraRepositorio _repo;
        public CrearReglaTiempoExtraCasoDeUso(IReglaTiempoExtraRepositorio repo) => _repo = repo;

        public async Task<ReglaTiempoExtra> EjecutarAsync(ReglaTiempoExtra regla)
        {
            var reglasActuales = await _repo.ObtenerPorRestauranteIdAsync(regla.RestauranteId);

            if (reglasActuales.Any(r => r.PorcentajeOcupacionHasta == regla.PorcentajeOcupacionHasta))
            {
                throw new ArgumentException($"Ya existe una regla configurada para el {regla.PorcentajeOcupacionHasta}%.");
            }

            return await _repo.CrearAsync(regla);
        }
    }
}
