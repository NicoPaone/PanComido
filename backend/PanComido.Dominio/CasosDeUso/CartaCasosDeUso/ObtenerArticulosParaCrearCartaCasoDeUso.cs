using PanComido.Dominio.Entidades;
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
                    plato.TiempoPreparacionEstimado = _tiempoDePreparacionPlatoServicio.CalcularTiempoPreparacionDinamico(plato);
                }
            }

            return articulosDb;
        }

        private decimal CalcularCostoDinamico(Articulo articulo)
        {
            if (articulo is Insumo bebida)
            {
                var ultimoPedido = bebida.PedidoInsumos?.LastOrDefault();
                return ultimoPedido?.PrecioCompra ?? 0;
            }

            if (articulo is Plato plato && plato.Ingredientes != null)
            {
                decimal costoPlato = 0;

                foreach (var ingredienteReceta in plato.Ingredientes)
                {
                    var ultimoPedido = ingredienteReceta.Insumo?.PedidoInsumos?.LastOrDefault();
                    var precioCompraInsumo = ultimoPedido?.PrecioCompra ?? 0;

                    costoPlato += precioCompraInsumo * ingredienteReceta.Cantidad;
                }

                return costoPlato;
            }

            return 0;
        }
    }
}