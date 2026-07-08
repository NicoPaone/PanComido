using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ReglaTiempoExtraCasosDeUso
{
    public class ModificarReglaTiempoExtraCasoDeUso
    {
        private readonly IReglaTiempoExtraRepositorio _repo;
        public ModificarReglaTiempoExtraCasoDeUso(IReglaTiempoExtraRepositorio repo) => _repo = repo;

        public async Task<ReglaTiempoExtra> EjecutarAsync(int id, ReglaTiempoExtra reglaActualizada)
        {
            var regla = await _repo.ObtenerPorIdAsync(id);
            if (regla == null || regla.RestauranteId != reglaActualizada.RestauranteId) 
            {
                throw new KeyNotFoundException("Regla no encontrada.");
            }

            var reglasActuales = await _repo.ObtenerPorRestauranteIdAsync(reglaActualizada.RestauranteId);
            if (reglasActuales.Any(r => r.Id != id && r.PorcentajeOcupacionHasta == reglaActualizada.PorcentajeOcupacionHasta))
            {
                throw new ArgumentException($"Ya existe otra regla configurada para el {reglaActualizada.PorcentajeOcupacionHasta}%.");
            }

            regla.PorcentajeOcupacionHasta = reglaActualizada.PorcentajeOcupacionHasta;
            regla.MinutosExtra = reglaActualizada.MinutosExtra;
            
            return await _repo.ActualizarAsync(regla);
        }
    }
}
