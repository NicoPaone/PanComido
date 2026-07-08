using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.CartaCasosDeUso
{
    public class ObtenerArticulosParaCrearCartaCasoDeUso
    {
        private readonly IArticuloRepositorio _articuloRepositorio;
        private readonly ITiempoDePreparacionPlatoServicio _tiempoDePreparacionPlatoServicio;

        public ObtenerArticulosParaCrearCartaCasoDeUso(IArticuloRepositorio articuloRepositorio,
                                                        ITiempoDePreparacionPlatoServicio tiempoDePreparacionPlatoServicio)
        {
            _articuloRepositorio = articuloRepositorio;
            _tiempoDePreparacionPlatoServicio = tiempoDePreparacionPlatoServicio;
        }

        public async Task<List<Articulo>> EjecutarAsync(int restauranteId)
        {
            var articulosDb = await _articuloRepositorio.ObtenerTodosLosArticulosParaCartaAsync(restauranteId);

            foreach (var art in articulosDb)
            {
                art.CostoCalculado = CalcularCostoDinamico(art);
                if (art is Plato plato)
                {
                    plato.TiempoPreparacionEstimado = await _tiempoDePreparacionPlatoServicio.CalcularTiempoPreparacionDinamico(plato);
                }
            }

            return articulosDb;
        }

        private decimal CalcularCostoDinamico(Articulo articulo)
        {
            if (articulo is Insumo bebida)
            {
                return ObtenerUltimoPrecioCompraRecibido(bebida);
            }

            if (articulo is Plato plato && plato.Ingredientes != null)
            {
                decimal costoPlato = 0;

                foreach (var ingredienteReceta in plato.Ingredientes)
                {
                    decimal precioCompraInsumo = ObtenerUltimoPrecioCompraRecibido(ingredienteReceta.Insumo);
                    costoPlato += precioCompraInsumo * ingredienteReceta.Cantidad;
                }

                return costoPlato;
            }

            if (articulo is BebidaPreparada bebidaPreparada && bebidaPreparada.Insumos != null)
            {
                decimal costoBebidaPreparada = 0;

                foreach (var itemReceta in bebidaPreparada.Insumos)
                {
                    decimal precioCompraInsumo = ObtenerUltimoPrecioCompraRecibido(itemReceta.Insumo);
                    costoBebidaPreparada += precioCompraInsumo * itemReceta.Cantidad;
                }

                return costoBebidaPreparada;
            }

            return 0;
        }

        private decimal ObtenerUltimoPrecioCompraRecibido(Insumo insumo)
        {
            var ultimoPedidoRecibido = insumo?.PedidoInsumos
                ?.Where(pi => pi.Estado == EstadoPedido.Recibido)
                .OrderByDescending(pi => pi.Fecha)
                .FirstOrDefault();

            return ultimoPedidoRecibido?.PrecioCompra ?? 0;
        }
    }
}