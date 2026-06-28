using Microsoft.Extensions.Logging;
using PanComido.Dominio.CasosDeUso.ArticuloCasosDeUso;
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
        private readonly IMesaRepositorio _mesaRepositorio;

        private readonly IDisponibilidadArticuloServicio _disponibilidadServicio;
        private readonly IGestionStockServicio _gestionDeStockServicio;

        private readonly IComandaNotificador _comandaNotificador;

        private readonly ILogger<ConfirmarPedidoClienteAComandaCasoDeUso> _logger;

        public ConfirmarPedidoClienteAComandaCasoDeUso(
            IComandaRepositorio comandaRepositorio,
            ILoteRepositorio loteRepositorio,
            IArticuloRepositorio articuloRepositorio,
            IMesaRepositorio mesaRepositorio,
            IDisponibilidadArticuloServicio disponibilidadServicio,
            IGestionStockServicio gestionDeStockServicio,
            IComandaNotificador comandaNotificador,
            ILogger<ConfirmarPedidoClienteAComandaCasoDeUso> logger)
        {
            _comandaRepositorio = comandaRepositorio;
            _loteRepositorio = loteRepositorio;
            _articuloRepositorio = articuloRepositorio;
            _mesaRepositorio = mesaRepositorio;
            _disponibilidadServicio = disponibilidadServicio;
            _gestionDeStockServicio = gestionDeStockServicio;
            _comandaNotificador = comandaNotificador;
            _logger = logger;
        }

        public async Task<Comanda> EjecutarAsync(int restauranteId, int comandaId, List<ArticuloComanda> articulosSolicitados)
        {
            _logger.LogInformation("Iniciando confirmación de pedido para la Comanda {ComandaId} en el Restaurante {RestauranteId}. Artículos solicitados: {CantidadArticulos}", comandaId, restauranteId, articulosSolicitados.Count);

            Comanda comanda = await ObtenerYValidarComandaAsync(comandaId);

            var stockInsumosDisponibles = await _loteRepositorio.ObtenerStockTotalDeInsumosDisponible(restauranteId, DateOnly.FromDateTime(DateTime.UtcNow));

            await ProcesarArticulosSolicitadosAsync(restauranteId, articulosSolicitados, stockInsumosDisponibles, comanda);

            ActualizarEstadoComanda(comanda);
            await _comandaRepositorio.ActualizarAsync(comanda);

            await _gestionDeStockServicio.DescontarStockPorArticulosAsync(restauranteId, articulosSolicitados);

            await NotificarCambiosAsync(comanda);

            _logger.LogInformation("Pedido confirmado y procesado exitosamente para la Comanda {ComandaId}. Se actualizaron inventarios y se emitieron notificaciones.", comandaId);

            return await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);
        }
        private async Task<Comanda> ObtenerYValidarComandaAsync(int comandaId)
        {
            Comanda comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);
            if (comanda == null || comanda.Estado == EstadoComanda.Finalizada)
            {
                _logger.LogWarning("Rechazo al confirmar pedido: La comanda {ComandaId} no existe o ya se encuentra finalizada.", comandaId);
                throw new InvalidOperationException("La comanda no existe o esta finalizada.");
            }
            return comanda;
        }
        private async Task ProcesarArticulosSolicitadosAsync(int restauranteId, List<ArticuloComanda> articulosSolicitados, Dictionary<int, decimal> stockInsumosDisponibles, Comanda comanda)
        {
            foreach (ArticuloComanda itemDeComanda in articulosSolicitados)
            {
                var articuloCompleto = await ObtenerYValidarArticuloAsync(restauranteId, comanda.Id, itemDeComanda);

                ValidarDisponibilidadDeStock(comanda.Id, articuloCompleto, itemDeComanda, stockInsumosDisponibles);
                itemDeComanda.Articulo = articuloCompleto;
                itemDeComanda.Entregado = false;
                DescontarStockEnMemoria(articuloCompleto, itemDeComanda, stockInsumosDisponibles);
                comanda.Items.Add(itemDeComanda);
            }
        }
        private async Task<Articulo> ObtenerYValidarArticuloAsync(int restauranteId, int comandaId, ArticuloComanda item)
        {
            var articuloCompleto = await _articuloRepositorio.ObtenerDetalleAsync(restauranteId, item.ArticuloId);
            if (articuloCompleto == null || !articuloCompleto.EsVisibleEnCarta)
            {
                _logger.LogWarning("Rechazo al confirmar pedido (Comanda {ComandaId}): El artículo {ArticuloId} no está disponible o no es visible en carta.", comandaId, item.ArticuloId);
                throw new ArgumentException($"El artículo con ID {item.ArticuloId} no está disponible.");
            }
            return articuloCompleto;
        }
        private void ValidarDisponibilidadDeStock(int comandaId, Articulo articuloCompleto, ArticuloComanda item, Dictionary<int, decimal> stockInsumosDisponibles)
        {
            bool estaDisponible = _disponibilidadServicio.VerificarDisponibilidad(articuloCompleto, item.Cantidad, stockInsumosDisponibles);
            if (!estaDisponible)
            {
                _logger.LogWarning("Rechazo al confirmar pedido (Comanda {ComandaId}): Quiebre de stock. No alcanza para preparar {Cantidad}x '{NombreArticulo}' (Id: {ArticuloId}).", comandaId, item.Cantidad, articuloCompleto.Nombre, item.ArticuloId);
                throw new InvalidOperationException($"No hay stock suficiente para preparar {item.Cantidad}x {articuloCompleto.Nombre}");
            }
        }
        private void DescontarStockEnMemoria(Articulo articuloCompleto, ArticuloComanda itemDeComanda, Dictionary<int, decimal> stockInsumosDisponibles)
        {
            if (articuloCompleto is Plato plato)
                DescontarStockDePlatoEnMemoria(plato, itemDeComanda, stockInsumosDisponibles);
            else
                DescontarStockDirectoEnMemoria(articuloCompleto, itemDeComanda, stockInsumosDisponibles);
        }
        private void DescontarStockDePlatoEnMemoria(Plato plato, ArticuloComanda itemDeComanda, Dictionary<int, decimal> stockInsumosDisponibles)
        {
            List<int> idIngredientesValidosDelPlato = plato.Ingredientes.Select(i => i.InsumoId).ToList();
            itemDeComanda.IngredientesExcluidosIds = itemDeComanda.IngredientesExcluidosIds
                .Where(id => idIngredientesValidosDelPlato.Contains(id))
                .Distinct()
                .ToList();
            foreach (var ingrediente in plato.Ingredientes)
            {
                bool ingredienteExcluido = itemDeComanda.IngredientesExcluidosIds.Contains(ingrediente.InsumoId);
                bool ingredienteSeEncuentraEnElStock = stockInsumosDisponibles.ContainsKey(ingrediente.InsumoId);

                if (!ingredienteExcluido && ingredienteSeEncuentraEnElStock)
                {
                    decimal cantidadARestar = ingrediente.Cantidad * itemDeComanda.Cantidad;
                    stockInsumosDisponibles[ingrediente.InsumoId] -= cantidadARestar;
                }
            }
        }
        private void DescontarStockDirectoEnMemoria(Articulo articulo, ArticuloComanda itemDeComanda, Dictionary<int, decimal> stockInsumosDisponibles)
        {
            itemDeComanda.IngredientesExcluidosIds = new List<int>();

            if (stockInsumosDisponibles.ContainsKey(articulo.Id))
            {
                stockInsumosDisponibles[articulo.Id] -= itemDeComanda.Cantidad;
            }
        }
        private void ActualizarEstadoComanda(Comanda comanda)
        {
            if (comanda.Estado != EstadoComanda.EnPreparacion && comanda.Estado != EstadoComanda.Nueva)
            {
                comanda.Estado = EstadoComanda.Nueva;
                comanda.HoraUltimoCambioEstado = DateTime.Now;
            }
        }
        private async Task NotificarCambiosAsync(Comanda comanda)
        {
            var mozosId = await _mesaRepositorio.ObtenerMozoIdsPorMesaAsync(comanda.MesaId);
            await _comandaNotificador.NotificarEstadoModificadoAsync(comanda, mozosId);
            await _comandaNotificador.NotificarComandaActualizadaAMesaAsync(comanda);
        }
    }
}
