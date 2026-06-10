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
        private readonly ICantidadDeMesasServicio _cantidadDeMesasServicio;

        public ObtenerArticulosParaCrearCartaCasoDeUso(IArticuloRepositorio articuloRepositorio,
                                                        ICantidadDeMesasServicio cantidadDeMesasServicio)
        {
            _articuloRepositorio = articuloRepositorio;
            _cantidadDeMesasServicio = cantidadDeMesasServicio;
        }

        public async Task<List<Articulo>> EjecutarAsync()
        {
            // 1. Traemos la lista cruda de la base de datos
            var articulosDb = await _articuloRepositorio.ObtenerTodosLosArticulosParaCartaAsync();

            // 2. Le calculamos el costo a cada uno "en el aire"
            foreach (var art in articulosDb)
            {
                art.CostoCalculado = CalcularCostoDinamico(art);
                if (art is Plato plato)
                {
                    plato.TiempoPreparacionBase = CalcularTiempoPreparacionDinamico(plato);
                }
            }

            // 3. Devolvemos las entidades de Dominio (Presentación se encargará de los DTOs)
            return articulosDb;
        }

        private decimal CalcularCostoDinamico(Articulo articulo)
        {
            if (articulo is Insumo bebida)
            {
                // Si es bebida, buscamos el último precio al que le compramos al proveedor
                var ultimoPedido = bebida.PedidoInsumos?.LastOrDefault();
                return ultimoPedido?.PrecioCompra ?? 0;
            }

            if (articulo is Plato plato && plato.Ingredientes != null)
            {
                decimal costoPlato = 0;

                foreach (var ingredienteReceta in plato.Ingredientes)
                {
                    // Buscamos a cuánto compramos el insumo de este ingrediente
                    var ultimoPedido = ingredienteReceta.Insumo?.PedidoInsumos?.LastOrDefault();
                    var precioCompraInsumo = ultimoPedido?.PrecioCompra ?? 0;

                    // Matemáticas: Precio de compra X Cantidad que lleva la receta
                    costoPlato += precioCompraInsumo * ingredienteReceta.Cantidad;
                }

                return costoPlato;
            }

            return 0;
        }

        private int CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas(int restauranteId)
        {
            int cantidadMesas = _cantidadDeMesasServicio.ObtenerCantidadDeMesasTotal(restauranteId);
            int mesasOcupadas = _cantidadDeMesasServicio.ObtenerCantidadDeMesasOcupadas(restauranteId);

            if (mesasOcupadas == 0)
            {
                return 0;
            } else if (mesasOcupadas <= cantidadMesas * 0.30)
            {
                return 5;
            } else if (mesasOcupadas <= cantidadMesas * 0.50)
            {
                return 10;
            } else if (mesasOcupadas <= cantidadMesas * 0.70)
            {
                return 15;
            }
            else
            {
                return 20;
            }
        }

        private int CalcularTiempoPreparacionDinamico(Plato plato)
        {
            return plato.TiempoPreparacionBase + CalcularTiempoExtraEnBaseALaOcupacionDeLasMesas(plato.RestauranteId);
        }
    }
}