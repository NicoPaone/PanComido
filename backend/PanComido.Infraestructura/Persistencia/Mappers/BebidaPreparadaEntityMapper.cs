using PanComido.Dominio.Entidades.Enums;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class BebidaPreparadaEntityMapper
    {
        public DOM.BebidaPreparada CompletarMapeoDominio(EF.Articulo efArticulo)
        {
            var insumos = efArticulo.BebidaPreparadum.BebidaPreparadaInsumos?.Select(bpi => new DOM.BebidaPreparadaInsumo
            {
                InsumoId = bpi.InsumoId,
                Cantidad = bpi.Cantidad,
                Insumo = new DOM.Insumo
                {
                    Id = bpi.InsumoId,
                    Nombre = bpi.Insumo?.IdArticuloNavigation?.Nombre ?? "Insumo sin nombre",
                    Categoria = bpi.Insumo?.CategoriaInsumo?.Descripcion,
                    PedidoInsumos = bpi.Insumo?.PedidoInsumos?.Select(pedido => new DOM.PedidoInsumo
                    {
                        InsumoId = pedido.InsumoId,
                        Cantidad = pedido.Cantidad,
                        PrecioCompra = pedido.PrecioCompra,
                        Fecha = pedido.Pedido?.Fecha ?? default,
                        Estado = pedido.Pedido != null ? (EstadoPedido)pedido.Pedido.EstadoPedidoId : default
                    }).ToList() ?? new List<DOM.PedidoInsumo>()
                }
            }).ToList() ?? new List<DOM.BebidaPreparadaInsumo>();

            bool tieneAlcohol = insumos.Any(i => i.Insumo?.Categoria == "Con alcohol");

            return new DOM.BebidaPreparada
            {
                Insumos = insumos,
                Categoria = tieneAlcohol ? "Con alcohol" : "Sin alcohol"
            };
        }


        public EF.BebidaPreparadum CompletarMapeoAEntidad(DOM.BebidaPreparada bebidaPreparadaDominio)
        {
            return new EF.BebidaPreparadum
            {
                BebidaPreparadaInsumos = bebidaPreparadaDominio.Insumos?.Select(i => new EF.BebidaPreparadaInsumo
                {
                    InsumoId = i.InsumoId,
                    Cantidad = i.Cantidad
                }).ToList() ?? new List<EF.BebidaPreparadaInsumo>()
            };
        }
    }
}
