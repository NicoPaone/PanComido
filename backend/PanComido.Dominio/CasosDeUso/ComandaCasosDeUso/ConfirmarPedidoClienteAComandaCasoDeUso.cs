using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ComandaCasosDeUso
{
    public class ConfirmarPedidoClienteAComandaCasoDeUso
    {
        private readonly IComandaRepositorio _comandaRepositorio;
        private readonly ILoteRepositorio _loteRepositorio;
        private readonly IArticuloRepositorio _articuloRepositorio;

        private readonly IDisponibilidadArticuloServicio _disponibilidadServicio;
        private readonly IGestionStockServicio _gestionDeStockServicio;

        public ConfirmarPedidoClienteAComandaCasoDeUso(
            IComandaRepositorio comandaRepositorio,
            ILoteRepositorio loteRepositorio,
            IArticuloRepositorio articuloRepositorio,
            IDisponibilidadArticuloServicio disponibilidadServicio,
            IGestionStockServicio gestionDeStockServicio)
        {
            _comandaRepositorio = comandaRepositorio;
            _loteRepositorio = loteRepositorio;
            _articuloRepositorio = articuloRepositorio;
            _disponibilidadServicio = disponibilidadServicio;
            _gestionDeStockServicio = gestionDeStockServicio;
        }

        public async Task<Comanda> EjecutarAsync(int restauranteId, int comandaId, List<ArticuloComanda> articulosSolicitados)
        {
            // determinar si me llega comandaid o mesaid.

            Comanda comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);
            if (comanda == null || comanda.Estado == EstadoComanda.Finalizada)
                throw new InvalidOperationException("La comanda no existe o esta finalizada.");

            var stockInsumosDisponibles = await _loteRepositorio.ObtenerStockTotalDeInsumosDisponible(restauranteId, DateOnly.FromDateTime(DateTime.UtcNow));

            foreach (ArticuloComanda item in articulosSolicitados)
            {
                var articuloCompleto = await _articuloRepositorio.ObtenerDetalleAsync(restauranteId, item.ArticuloId);

                if (articuloCompleto == null || !articuloCompleto.EsVisibleEnCarta)
                    throw new ArgumentException($"El artículo con ID {item.ArticuloId} no está disponible.");

                bool estaDisponible = _disponibilidadServicio.VerificarDisponibilidad(articuloCompleto, item.Cantidad, stockInsumosDisponibles);

                if (!estaDisponible)
                    throw new InvalidOperationException($"No hay stock suficiente para preparar {item.Cantidad}x {articuloCompleto.Nombre}");

                item.Articulo = articuloCompleto;
                item.Entregado = false;


                if (articuloCompleto is Plato plato)
                {
                    foreach (var recetaItem in plato.Ingredientes)
                    {
                        if (stockInsumosDisponibles.ContainsKey(recetaItem.InsumoId))
                        {
                            decimal cantidadARestar = recetaItem.Cantidad * item.Cantidad;
                            stockInsumosDisponibles[recetaItem.InsumoId] -= cantidadARestar;
                        }
                    }
                }
                else // bebida por descarte
                {
                    if (stockInsumosDisponibles.ContainsKey(articuloCompleto.Id))
                        stockInsumosDisponibles[articuloCompleto.Id] -= item.Cantidad;
                }

                comanda.Items.Add(item);
            }
            
            if (comanda.Estado != EstadoComanda.EnPreparacion)
            {
                comanda.Estado = EstadoComanda.Nueva;
                comanda.HoraUltimoCambioEstado = DateTime.Now;
            }

            await _comandaRepositorio.ActualizarAsync(comanda);

            // despues implementar signal R para notificar a cocina y mozo el cambio de estado.
            
            await _gestionDeStockServicio.DescontarStockPorArticulosAsync(restauranteId, articulosSolicitados);
            // signal R para actualizar stock en tiempo real?

            return comanda;
        }
    }
}
