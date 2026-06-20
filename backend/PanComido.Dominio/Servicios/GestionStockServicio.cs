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
                        bool ingredienteExcluidoPorElCliente = itemComanda.IngredientesExcluidosIds.Contains(itemReceta.InsumoId);
                        if (!ingredienteExcluidoPorElCliente)
                            AcumularInsumoARestar(insumosARestar, itemReceta.InsumoId, itemReceta.Cantidad * itemComanda.Cantidad);
                    }
                }
                else if (itemComanda.Articulo is Insumo bebida)
                    AcumularInsumoARestar(insumosARestar, bebida.Id, itemComanda.Cantidad);
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
                    if (cantidadPorDescontar <= 0)
                        break;

                    decimal aDescontarDeEsteLote = Math.Min(cantidadPorDescontar, lote.Cantidad);

                    lote.Cantidad -= aDescontarDeEsteLote;
                    cantidadPorDescontar -= aDescontarDeEsteLote;

                    lotesModificados.Add(lote);
                }
            }

            if (lotesModificados.Any())
                await _loteRepositorio.ActualizarLotesAsync(lotesModificados);
        }
        private void AcumularInsumoARestar(Dictionary<int, decimal> diccionario, int insumoId, decimal cantidadASumar)
        {
            if (!diccionario.ContainsKey(insumoId))
            {
                diccionario[insumoId] = 0;
            }

            diccionario[insumoId] += cantidadASumar;
        }
    }

}
