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

        public TiempoDePreparacionPlatoServicio(IMesaRepositorio mesaRepositorio)
        {
            _mesaRepositorio = mesaRepositorio;
        }

        public async Task<int> CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas(int restauranteId)
        {
            int cantidadMesas = (await _mesaRepositorio.ObtenerTodasAsync(restauranteId)).Count;
            int mesasOcupadas = (await _mesaRepositorio.ObtenerOcupadasAsync(restauranteId)).Count;

            if (mesasOcupadas == 0)
            {
                return 0;
            }
            else if (mesasOcupadas <= cantidadMesas * 0.30)
            {
                return 5;
            }
            else if (mesasOcupadas <= cantidadMesas * 0.50)
            {
                return 10;
            }
            else if (mesasOcupadas <= cantidadMesas * 0.70)
            {
                return 15;
            }
            else
            {
                return 20;
            }
        }

        public async Task<int> CalcularTiempoPreparacionDinamico(Plato plato)
        {
            return plato.TiempoPreparacionBase + await CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas(plato.RestauranteId);
        }
    }
}
