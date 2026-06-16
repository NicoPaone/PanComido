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

        public ConfirmarPedidoClienteAComandaCasoDeUso(
            IComandaRepositorio comandaRepositorio,
            ILoteRepositorio loteRepositorio,
            IArticuloRepositorio articuloRepositorio,
            IMesaRepositorio mesaRepositorio,
            IDisponibilidadArticuloServicio disponibilidadServicio,
            IGestionStockServicio gestionDeStockServicio,
            IComandaNotificador comandaNotificador)
        {
            _comandaRepositorio = comandaRepositorio;
            _loteRepositorio = loteRepositorio;
            _articuloRepositorio = articuloRepositorio;
            _mesaRepositorio = mesaRepositorio;
            _disponibilidadServicio = disponibilidadServicio;
            _gestionDeStockServicio = gestionDeStockServicio;
            _comandaNotificador = comandaNotificador;
        }

        public async Task<Comanda> EjecutarAsync(int restauranteId, int comandaId, List<ArticuloComanda> articulosSolicitados)
        {

            Comanda comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);
            if (comanda == null || comanda.Estado == EstadoComanda.Finalizada)
                throw new InvalidOperationException("La comanda no existe o esta finalizada.");

            var stockInsumosDisponibles = await _loteRepositorio.ObtenerStockTotalDeInsumosDisponible(restauranteId, DateOnly.FromDateTime(DateTime.UtcNow));

            // TODO: aca ver si tambien no validar los ingredientes opcionales que saco, sacar de la lista a los articulos que saco el comensal
            // basicamente validar lo que el usuario pidio, no demas

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


            var mozosId = await _mesaRepositorio.ObtenerMozoIdsPorMesaAsync(comanda.MesaId);
            // signalR
            await _comandaNotificador.NotificarEstadoModificadoAsync(comanda, mozosId);
            await _comandaNotificador.NotificarComandaActualizadaAMesaAsync(comanda);


            await _gestionDeStockServicio.DescontarStockPorArticulosAsync(restauranteId, articulosSolicitados);
            
            // signal R para actualizar stock en tiempo real?

            return comanda;
        }
    }
}
