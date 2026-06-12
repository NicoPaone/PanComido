using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso
{
    public class ActualizarDatosDelLocalCasoDeUso
    {
        private readonly IRestauranteRepositorio _restauranteRepositorio;

        public ActualizarDatosDelLocalCasoDeUso(IRestauranteRepositorio restauranteRepositorio) 
        {
            _restauranteRepositorio = restauranteRepositorio;
        }

        public async Task<Restaurante> EjecutarAsync(int restauranteId, Restaurante restauranteDatos) 
        {
            await _restauranteRepositorio.ActualizarDatosDelLocalAsync(restauranteId, restauranteDatos);

            return await _restauranteRepositorio.ObtenerDatosDelLocalAsync(restauranteId);
        }
    }
}
