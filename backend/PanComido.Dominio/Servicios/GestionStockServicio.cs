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
            Dictionary<int, decimal> insumosARestar = CalcularInsumosARestar(articulosSolicitados);

            if (insumosARestar.Any())
                await DescontarPorLotesSegunFIFOAsync(restauranteId, insumosARestar);
        }

        private Dictionary<int, decimal> CalcularInsumosARestar(List<ArticuloComanda> articulosSolicitados)
        {
            Dictionary<int, decimal> insumosARestar = new();

            foreach (var itemDeComanda in articulosSolicitados)
            {
                if (itemDeComanda.Articulo is Plato plato)
                    CalcularInsumosDePlato(plato, itemDeComanda, insumosARestar);
                else if (itemDeComanda.Articulo is BebidaPreparada bebidaPreparada)
                    CalcularInsumosDeBebidaPreparada(bebidaPreparada, itemDeComanda, insumosARestar);
                else
                    CalcularInsumoDirecto(itemDeComanda.Articulo, itemDeComanda, insumosARestar);
            }

            return insumosARestar;
        }

        private void CalcularInsumosDePlato(Plato plato, ArticuloComanda itemDeComanda, Dictionary<int, decimal> insumosARestar)
        {
            foreach (var ingrediente in plato.Ingredientes)
            {
                bool ingredienteExcluido = itemDeComanda.IngredientesExcluidosIds.Contains(ingrediente.InsumoId);

                if (!ingredienteExcluido)
                {
                    decimal cantidadTotalARestar = ingrediente.Cantidad * itemDeComanda.Cantidad;
                    AcumularInsumoARestar(insumosARestar, ingrediente.InsumoId, cantidadTotalARestar);
                }
            }
        }

        private void CalcularInsumosDeBebidaPreparada(BebidaPreparada bebidaPreparada, ArticuloComanda itemDeComanda, Dictionary<int, decimal> insumosARestar)
        {
            foreach (var item in bebidaPreparada.Insumos)
            {
                decimal cantidadTotalARestar = item.Cantidad * itemDeComanda.Cantidad;
                AcumularInsumoARestar(insumosARestar, item.InsumoId, cantidadTotalARestar);
            }
        }

        private void CalcularInsumoDirecto(Articulo articulo, ArticuloComanda itemDeComanda, Dictionary<int, decimal> insumosARestar)
        {
            AcumularInsumoARestar(insumosARestar, articulo.Id, itemDeComanda.Cantidad);
        }


        private void AcumularInsumoARestar(Dictionary<int, decimal> insumosARestar, int insumoId, decimal cantidadASumar)
        {
            if (!insumosARestar.ContainsKey(insumoId))
            {
                insumosARestar[insumoId] = 0;
            }

            insumosARestar[insumoId] += cantidadASumar;
        }

        private async Task DescontarPorLotesSegunFIFOAsync(int restauranteId, Dictionary<int, decimal> insumosARestar)
        {
            List<Lote> lotesModificados = new();
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
            {
                await _loteRepositorio.ActualizarLotesAsync(lotesModificados);
            }
        }
    }

}
