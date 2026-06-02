using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
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

        public ObtenerDetalleArticuloCasoDeUso(IArticuloRepositorio articuloRepositorio)
        {
            _articuloRepositorio = articuloRepositorio;
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

            return articulo;
        }
    }
}
