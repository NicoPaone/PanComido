using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.Dashboard
{
    public  class ObtenerVencimientosYCriticidadDashboardCasoDeUso
    {
        private readonly IInsumoRepositorio _insumoRepositorio;


        public ObtenerVencimientosYCriticidadDashboardCasoDeUso(IInsumoRepositorio insumoRepositorio)
        {
            _insumoRepositorio = insumoRepositorio;
        }


        public async Task<List<Insumo>> EjecutarAsync(int restauranteId)
        {
            var insumos = await _insumoRepositorio.ObtenerInsumosProximosAVencerAsync(restauranteId);
            var hoy = DateOnly.FromDateTime(DateTime.Now);

            foreach (var insumo in insumos)
            {
                if (insumo.Vencimiento.HasValue)
                {
                    insumo.CriticidadVencimiento = CalcularCriticidad(hoy, insumo.Vencimiento.Value);
                }
            }

            return insumos;
        }

        private CriticidadVencimiento? CalcularCriticidad(DateOnly hoy, DateOnly vencimiento)
        {
            var diasRestantes = vencimiento.DayNumber - hoy.DayNumber;

            if (diasRestantes <= 2) return CriticidadVencimiento.Alta;
            if (diasRestantes <= 5) return CriticidadVencimiento.Media;
            if (diasRestantes <= 10) return CriticidadVencimiento.Baja;

            return null;
        }



    }
}
