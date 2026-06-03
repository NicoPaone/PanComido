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
    public class GestionStockServicio : IGestionStockServicio
    {
        private readonly ILoteRepositorio _loteRepositorio;

        public GestionStockServicio(ILoteRepositorio loteRepositorio)
        {
            _loteRepositorio = loteRepositorio;
        }

        public async Task DescontarStockPorArticulosAsync(int restauranteId, List<ArticuloComanda> articulosSolicitados)
        {
            Dictionary<int, decimal> insumosARestar = new();

            foreach (var itemComanda in articulosSolicitados)
            {
                if (itemComanda.Articulo is Plato plato)
                {
                    foreach (var itemReceta in plato.Ingredientes)
                    {
                        // TODO: A futuro para descontar el stock de ingredientes opcionales
                        // chequear si el cliente destildo este ingrediente para no restarlo.

                        if (!insumosARestar.ContainsKey(itemReceta.InsumoId))
                            insumosARestar[itemReceta.InsumoId] = 0;

                        insumosARestar[itemReceta.InsumoId] += itemReceta.Cantidad * itemComanda.Cantidad;
                    }
                }
                else if (itemComanda.Articulo is Insumo bebida)
                {
                    if (!insumosARestar.ContainsKey(bebida.Id))
                        insumosARestar[bebida.Id] = 0;

                    insumosARestar[bebida.Id] += itemComanda.Cantidad;
                }
            }

            List<Lote> lotesModificados = new();

            // FIFO: Descontar de los lotes ordenados por vencimiento
            foreach (var kvp in insumosARestar)
            {
                int insumoId = kvp.Key;
                decimal cantidadPorDescontar = kvp.Value;

                List<Lote> lotesDisponibles = await _loteRepositorio.ObtenerLotesPorFechaVencimientoAscendenteAsync(restauranteId, insumoId);

                foreach (var lote in lotesDisponibles)
                {
                    // si ya descontamos todo lo que necesitamos, salimos del loop de lotes
                    if (cantidadPorDescontar <= 0)
                        break;

                    // el lote tiene suficiente stock para cubrir todo el pedido
                    if (lote.Cantidad >= cantidadPorDescontar)
                    {
                        lote.Cantidad -= cantidadPorDescontar;
                        cantidadPorDescontar = 0;
                        lotesModificados.Add(lote);
                    }
                    else // lote no tiene suficiente stock, lo vaciamos y seguimos descontando del proximo lote
                    {
                        cantidadPorDescontar -= lote.Cantidad;
                        lote.Cantidad = 0;
                        lotesModificados.Add(lote);
                    }
                }
            }

            if (lotesModificados.Any())
                await _loteRepositorio.ActualizarLotesAsync(lotesModificados);
        }
    }
}
