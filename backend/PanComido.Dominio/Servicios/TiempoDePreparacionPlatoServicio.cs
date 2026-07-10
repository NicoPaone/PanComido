using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Servicios
{
    public class TiempoDePreparacionPlatoServicio : ITiempoDePreparacionPlatoServicio
    {
        private readonly IMesaRepositorio _mesaRepositorio;
        private readonly IReglaTiempoExtraRepositorio _reglaTiempoExtraRepositorio;

        public TiempoDePreparacionPlatoServicio(IMesaRepositorio mesaRepositorio, IReglaTiempoExtraRepositorio reglaTiempoExtraRepositorio)
        {
            _mesaRepositorio = mesaRepositorio;
            _reglaTiempoExtraRepositorio = reglaTiempoExtraRepositorio;
        }

        public async Task<int> CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas(int restauranteId)
        {
            int cantidadMesas = (await _mesaRepositorio.ObtenerTodasAsync(restauranteId)).Count;
            int mesasOcupadas = (await _mesaRepositorio.ObtenerOcupadasAsync(restauranteId)).Count;

            if (mesasOcupadas == 0 || cantidadMesas == 0) return 0;

            int porcentajeOcupacion = (mesasOcupadas * 100) / cantidadMesas;

            List<ReglaTiempoExtra> reglas = await _reglaTiempoExtraRepositorio.ObtenerPorRestauranteIdAsync(restauranteId);

            if (!reglas.Any()) return 0;

            List<ReglaTiempoExtra> reglasOrdenadas = reglas.OrderBy(r => r.PorcentajeOcupacionHasta).ToList();

            foreach (ReglaTiempoExtra regla in reglasOrdenadas)
            {
                if (porcentajeOcupacion <= regla.PorcentajeOcupacionHasta) return regla.MinutosExtra;
            }
           
            return reglasOrdenadas.Last().MinutosExtra;
        }

        public async Task<int> CalcularTiempoPreparacionDinamico(Plato plato)
        {
            return plato.TiempoPreparacionBase + await CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas(plato.RestauranteId);
        }
    }
}
