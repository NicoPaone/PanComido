using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ArticuloCasosDeUso
{
    public class ObtenerDetalleArticuloCasoDeUso
    {
        private readonly IArticuloRepositorio _articuloRepositorio;
        private readonly ILoteRepositorio _loteRepositorio;

        private readonly IDisponibilidadArticuloServicio _disponibilidadServicio;

        public ObtenerDetalleArticuloCasoDeUso(
            IArticuloRepositorio articuloRepositorio, 
            IDisponibilidadArticuloServicio disponibilidadServicio
            ,
            ILoteRepositorio loteRepositorio)
        {
            _articuloRepositorio = articuloRepositorio;
            _disponibilidadServicio = disponibilidadServicio;
            _loteRepositorio = loteRepositorio;
        }

        public async Task<Articulo> EjecutarAsync(int restauranteId, int articuloId)
        {
            Articulo articulo = await _articuloRepositorio.ObtenerDetalleAsync(restauranteId, articuloId);

            if (articulo == null)
                throw new ArgumentException("El artículo no existe o no pertenece al restaurante.");

            if (!articulo.EsVisibleEnCarta)
            {
                throw new ArgumentException("El artículo solicitado no está disponible en la carta.");
            }

            Dictionary<int, decimal> stockInsumosDisponibles = await _loteRepositorio.ObtenerStockTotalDeInsumosDisponible(restauranteId, DateOnly.FromDateTime(DateTime.UtcNow));

            bool sePuedeProducir = _disponibilidadServicio.VerificarDisponibilidad(articulo, stockInsumosDisponibles);
            
            if (!sePuedeProducir)
            {
                throw new ArgumentException("El artículo solicitado no está disponible actualmente debido a la falta de insumos.");
            }

            return articulo;
        }
    }
}
