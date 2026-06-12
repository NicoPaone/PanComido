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
            
            var insumos = await _insumoRepositorio.ObtenerInsumosConLotesAsync(restauranteId);
            var hoy = DateOnly.FromDateTime(DateTime.Now);

            foreach (var insumo in insumos)
            {
                // Buscamos la fecha de vencimiento más próxima de los lotes
                var fechaMasProxima = insumo.Lotes
                    .Where(l => l.FechaVencimiento.HasValue)
                    .OrderBy(l => l.FechaVencimiento)
                    .Select(l => l.FechaVencimiento)
                    .FirstOrDefault();

                if (fechaMasProxima.HasValue)
                {
                    insumo.Vencimiento = fechaMasProxima;
                    insumo.CriticidadVencimiento = CalcularCriticidad(hoy, fechaMasProxima.Value);
                }
            }

            // Filtramos los que tienen vencimiento y los ordenamos por fecha
            return insumos.Where(i => i.Vencimiento.HasValue)
                          .OrderBy(i => i.Vencimiento)
                          .ToList();
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
