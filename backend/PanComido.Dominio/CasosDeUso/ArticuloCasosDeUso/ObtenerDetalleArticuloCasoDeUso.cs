using Microsoft.Extensions.Logging;
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

        private readonly ILogger<ObtenerDetalleArticuloCasoDeUso> _logger;

        public ObtenerDetalleArticuloCasoDeUso(
            IArticuloRepositorio articuloRepositorio, 
            IDisponibilidadArticuloServicio disponibilidadServicio,
            ILoteRepositorio loteRepositorio,
            ILogger<ObtenerDetalleArticuloCasoDeUso> logger)
        {
            _articuloRepositorio = articuloRepositorio;
            _disponibilidadServicio = disponibilidadServicio;
            _loteRepositorio = loteRepositorio;
            _logger = logger;
        }

        public async Task<Articulo> EjecutarAsync(int restauranteId, int articuloId)
        {
            Articulo articulo = await _articuloRepositorio.ObtenerDetalleAsync(restauranteId, articuloId);

            if (articulo == null)
            {
                _logger.LogWarning("Intento de obtener detalle de un artículo inexistente o ajeno al local. RestauranteId: {RestauranteId}, ArticuloId: {ArticuloId}", restauranteId, articuloId);
                throw new ArgumentException("El artículo no existe o no pertenece al restaurante.");
            }

            if (!articulo.EsVisibleEnCarta)
            {
                _logger.LogWarning("Intento de acceder a un artículo oculto en la carta. RestauranteId: {RestauranteId}, ArticuloId: {ArticuloId}", restauranteId, articuloId);
                throw new ArgumentException("El artículo solicitado no está disponible en la carta.");
            }

            Dictionary<int, decimal> stockInsumosDisponibles = await _loteRepositorio.ObtenerStockTotalDeInsumosDisponible(restauranteId, DateOnly.FromDateTime(DateTime.UtcNow));

            bool sePuedeProducir = _disponibilidadServicio.VerificarDisponibilidad(articulo, stockInsumosDisponibles);
            
            if (!sePuedeProducir)
            {
                _logger.LogWarning("Intento de acceder a un artículo sin stock suficiente para su producción. RestauranteId: {RestauranteId}, ArticuloId: {ArticuloId}", restauranteId, articuloId);
                throw new ArgumentException("El artículo solicitado no está disponible actualmente debido a la falta de insumos.");
            }

            return articulo;
        }
    }
}
