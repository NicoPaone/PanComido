using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class ObtenerDatosMesaBienvenidaCasoDeUso
    {
        private readonly IRestauranteRepositorio _restauranteRepositorio;
        private readonly IMesaRepositorio _mesaRepositorio;

        public ObtenerDatosMesaBienvenidaCasoDeUso(IRestauranteRepositorio restauranteRepositorio, IMesaRepositorio mesaRepositorio)
        {
            _restauranteRepositorio = restauranteRepositorio;
            _mesaRepositorio = mesaRepositorio;
        }

        public async Task<BienvenidaMesaDatos> EjecutarAsync(int mesaId, int restauranteId)
        {
            Restaurante restauranteDatos = await _restauranteRepositorio.ObtenerDatosDelLocalAsync(restauranteId);

            if (restauranteDatos == null)
                throw new KeyNotFoundException("Restaurante no encontrado");

            MesaConPosiciones mesa = await _mesaRepositorio.ObtenerPorIdAsync(mesaId, restauranteId);

            if (mesa == null)
                throw new KeyNotFoundException("Mesa no encontrada");

            if (mesa.EstadoMesa == EstadoMesa.Reservada || mesa.EstadoMesa == EstadoMesa.Deshabilitada)
                throw new InvalidOperationException($"Esta mesa se encuentra {mesa.EstadoMesa.ToString()}");

            if (mesa.EstadoMesa == EstadoMesa.Ocupada)
                throw new InvalidOperationException("La mesa ya está ocupada, puede pedirle al comensal anfitrion el QR de invitacion");

            return new BienvenidaMesaDatos
            {
                Mesa = mesa,
                RestauranteDatos = restauranteDatos
            };
        }
    }
}
