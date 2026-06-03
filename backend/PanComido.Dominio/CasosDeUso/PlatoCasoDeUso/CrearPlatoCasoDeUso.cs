using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.PlatoCasosDeUso
{
    public  class CrearPlatoCasoDeUso
    {
        private readonly IPlatoRepositorio _platoRepositorio;

        public CrearPlatoCasoDeUso(IPlatoRepositorio platoRepositorio)
        {
            _platoRepositorio = platoRepositorio;
        }   

        public async Task EjecutarAsync ( int restauranteID, Plato plato)
        {
            if ( string.IsNullOrWhiteSpace(plato.Nombre) )
            {
                throw new ArgumentException("El nombre del plato no puede estar vacío.");
            }
            if (plato.PrecioVentaFinal <= 0)
            {
                throw new ArgumentException("El precio de venta final debe ser mayor que cero.");
            }

            if ( plato.Ingredientes == null || !plato.Ingredientes.Any() )
            {
                throw new ArgumentException("El plato debe tener al menos un ingrediente.");
            }
            plato.RestauranteId = restauranteID;   

            await _platoRepositorio.CrearAsync(plato);
        }
    }
}
